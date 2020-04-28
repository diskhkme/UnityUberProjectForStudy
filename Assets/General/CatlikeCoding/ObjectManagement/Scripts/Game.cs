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
            storage.Save(this); //현재 이 save는 Game 컴포넌트가 달려있는 게임오브젝트의 transform을 저장할 뿐임!
        }
        else if(Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
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
        shapes.Add(instance);
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(-saveVersion); //버전도 함께 저장, 이전 버전에는 version 정보가 없으므로, count와 구분을 위해 음수로 저장함
        writer.Write(shapes.Count);
        for(int i=0;i<shapes.Count;i++)
        {
            writer.Write(shapes[i].ShapeId);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = -reader.ReadInt(); //이전 버전 세이브 파일을 읽을 수 있게 하려고 했지만, 이전 버전에는 version 데이터가 아예 없음
        if(version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
        }

        int count = version <= 0 ? -version : reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }

}
