using System.Collections.Generic;
using UnityEngine;

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


    List<Shape> shapes;

    //수정 이전의 save 파일(shape id가 없는 파일)도 로드할 수 있는 기능이 필요할 수 있음!
    const int saveVersion = 1; 

    private void Awake()
    {
        shapes = new List<Shape>();
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
            Destroy(shapes[i].gameObject);
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
            Destroy(shapes[index].gameObject);

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
