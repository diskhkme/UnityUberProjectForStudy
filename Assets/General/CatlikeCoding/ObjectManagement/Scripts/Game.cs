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
        writer.Write(shapes.Count);
        for(int i=0;i<shapes.Count;i++)
        {
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            Shape o = shapeFactory.Get(0);
            o.Load(reader);
            shapes.Add(o);
        }
    }

}
