using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Game 클래스 자체를 persistableObject 상속하도록 바꿈!
public class Game : PersistableObject
{
    public ShapeFactory shapeFactory;
    public PersistentStorage storage;

    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;
    public KeyCode destroyKey = KeyCode.X;

    public float CreationSpeed { get; set; }
    public float DestructionSpeed { get; set; }
    float creationProgress;
    float destructionProgress;

    public int levelCount;
    int loadLevelBuildIndex;


    List<Shape> shapes;

    //수정 이전의 save 파일(shape id가 없는 파일)도 로드할 수 있는 기능이 필요할 수 있음!
    const int saveVersion = 2; 

    private void Start()
    {
        shapes = new List<Shape>();

        if(Application.isEditor)
        {
            for(int i=0;i<SceneManager.sceneCount;i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if(loadedScene.name.Contains("Level ")) //main scene과 병행해서 열리는 level이라면
                {
                    SceneManager.SetActiveScene(loadedScene);
                    loadLevelBuildIndex = loadedScene.buildIndex;
                    return;
                }
            }
        }
        
        StartCoroutine(LoadLevel(1));
    }

    IEnumerator LoadLevel(int levelBuildIndex)
    {
        this.enabled = false;
        if(loadLevelBuildIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadLevelBuildIndex);
        }
        //두 개의 Scene을 동시에 열기 위해서는 매뉴얼 작업이 필요함 (현재 상태에서, 게임 전체를 관리하는 Main Scene과, 특정 level에 종속된 Level1 scene이 있음.
        //따라서, 동시에 열려면 "Game" gameobject가 있는 Main Scene에서 Level 1의 load가 필요. LoadSceneMode.additive 필요
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive); //async loading. 문제는 로딩이 다 되기 전에도 update가 호출될 수 있다는 것. enabled로 제어해 주어야 함
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex)); //directional light를 level1에 넣어놨기 때문에 active scene을 바꾸어야 함....바꾸려면 시간이 걸리기 떄문에 코루틴으로 작성
        loadLevelBuildIndex = levelBuildIndex;
        this.enabled = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(createKey))
        {
            CreateShape();
        }
        else if(Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }
        else if(Input.GetKeyDown(saveKey))
        {
            storage.Save(this, saveVersion); //현재 이 save는 Game 컴포넌트가 달려있는 게임오브젝트의 transform을 저장할 뿐임!
        }
        else if(Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
        else if(Input.GetKeyDown(destroyKey))
        {
            DestroyShape();
        }
        else
        {
            for(int i=1;i<=levelCount;i++)
            {
                if(Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }

        creationProgress += Time.deltaTime * CreationSpeed;
        //if(creationProgress >= 1f)
        while(creationProgress >= 1f) //creationprogress 가 2 이상으로 커질 수도 있음을 감안하면,
        {
            //creationProgress = 0f;
            creationProgress -= 1f;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f) //creationprogress 가 2 이상으로 커질 수도 있음을 감안하면,
        {
            destructionProgress -= 1f;
            DestroyShape();
        }
    }

    private void BeginNewGame()
    {
        for(int i=0;i<shapes.Count;i++)
        {
            //Destroy(shapes[i].gameObject);
            shapeFactory.Reclaim(shapes[i]);
        }
        shapes.Clear();
    }

    void CreateShape()
    {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        instance.SetColor(Random.ColorHSV(hueMin: 0f, hueMax: 1f, 
                                           saturationMin: 0.5f, saturationMax: 1f, 
                                           valueMin: 0.25f, valueMax: 1f, 
                                           alphaMin: 1f, alphaMax: 1f));
        shapes.Add(instance);
    }

    void DestroyShape()
    {
        if(shapes.Count>0)
        {
            int index = Random.Range(0, shapes.Count);
            //Destroy(shapes[index].gameObject);
            shapeFactory.Reclaim(shapes[index]);

            //List에서 객체를 제거할때, 빈 칸을 채우는 shift가 일어나지 않도록 하기 위해 지울 index ref와 마지막 index ref를 스왑한 후 삭제
            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
        
    }

    public override void Save(GameDataWriter writer)
    {
        //writer.Write(-saveVersion); //버전도 함께 저장, 이전 버전에는 version 정보가 없으므로, count와 구분을 위해 음수로 저장함 --> persistent storage에서 수행
        writer.Write(shapes.Count);
        writer.Write(loadLevelBuildIndex); //이제는 어떤 scene에 연관된 object들인지까지 구분해서 저장
        for(int i=0;i<shapes.Count;i++)
        {
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version; //이전 버전 세이브 파일을 읽을 수 있게 하려고 했지만, 이전 버전에는 version 데이터가 아예 없음
        if(version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
        }

        int count = version <= 0 ? -version : reader.ReadInt();
        StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt())); //로드 할때는 저장된 level까지 로드
        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId, materialId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }

}
