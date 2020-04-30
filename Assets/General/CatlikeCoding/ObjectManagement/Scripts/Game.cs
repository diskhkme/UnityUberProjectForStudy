using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : PersistableObject
{
    public static Game Instance { get; private set; } 

    [SerializeField] ShapeFactory shapeFactory;
    [SerializeField] PersistentStorage storage;

    [SerializeField] KeyCode createKey = KeyCode.C;
    [SerializeField] KeyCode newGameKey = KeyCode.N;
    [SerializeField] KeyCode saveKey = KeyCode.S;
    [SerializeField] KeyCode loadKey = KeyCode.L;
    [SerializeField] KeyCode destroyKey = KeyCode.X;

    public SpawnZone spawnZoneOfLevel { get; set; }
    [SerializeField] float CreationSpeed { get; set; }
    [SerializeField] float DestructionSpeed { get; set; }
    float creationProgress;
    float destructionProgress;
    Random.State mainRandomState;
    [SerializeField] bool reseedOnLoad; //게임을 시작할때 seed를 새로 할당할 것인지, 재사용할 것인지를 optional로 둠

    [SerializeField] int levelCount;
    int loadLevelBuildIndex;
    
    List<Shape> shapes;

    const int saveVersion = 3;

    private void OnEnable()
    {
        Instance = this;
    }

    private void Start()
    {
        //새 게임을 시작하면 완전히 새로운 random sequence를 사용하도록
        mainRandomState = Random.state;

        shapes = new List<Shape>();

        if(Application.isEditor)
        {
            for(int i=0;i<SceneManager.sceneCount;i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if(loadedScene.name.Contains("Level "))
                {
                    SceneManager.SetActiveScene(loadedScene);
                    loadLevelBuildIndex = loadedScene.buildIndex;
                    return;
                }
            }
        }

        BeginNewGame();
        StartCoroutine(LoadLevel(1));
    }

    IEnumerator LoadLevel(int levelBuildIndex)
    {
        this.enabled = false;
        if(loadLevelBuildIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadLevelBuildIndex);
        }
        
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
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
            storage.Save(this, saveVersion);
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
        while(creationProgress >= 1f) 
        {
            creationProgress -= 1f;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestroyShape();
        }
    }

    private void BeginNewGame()
    {
        Random.state = mainRandomState;
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledDeltaTime; //bitwise or과 time을 사용해 랜덤 강화
        mainRandomState = Random.state;
        Random.InitState(seed);

        for(int i=0;i<shapes.Count;i++)
        {
            shapeFactory.Reclaim(shapes[i]);
        }
        shapes.Clear();
    }

    void CreateShape()
    {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = spawnZoneOfLevel.SpawnPoint;
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
            shapeFactory.Reclaim(shapes[index]);

            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
        
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(shapes.Count);
        writer.Write(Random.state); //Random State를 함께 저장
        writer.Write(loadLevelBuildIndex); 
        for(int i=0;i<shapes.Count;i++)
        {
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if(version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
        }

        int count = version <= 0 ? -version : reader.ReadInt();

        if(version >= 3)
        {
            Random.State state = reader.ReadRandomState();
            if(!reseedOnLoad)
            {
                Random.state = state; //reseed를 하지 않는다는 이야기는 저장된 seed를 사용하겠다는 뜻
            }
        }

        StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));
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
