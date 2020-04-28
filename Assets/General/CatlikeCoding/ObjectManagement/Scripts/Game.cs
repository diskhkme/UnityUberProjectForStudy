using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Transform prefab;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;

    List<Transform> objects;

    string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
        objects = new List<Transform>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(createKey))
        {
            CreateObject();
        }
        else if(Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }
        else if(Input.GetKeyDown(saveKey))
        {
            Save();
        }
        else if(Input.GetKeyDown(loadKey))
        {
            Load();
        }
    }

    private void BeginNewGame()
    {
        for(int i=0;i<objects.Count;i++)
        {
            Destroy(objects[i].gameObject);
        }
        objects.Clear();
    }

    void CreateObject()
    {
        Transform t = Instantiate(prefab);
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        objects.Add(t);
    }

    void Save()
    {
        using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            //BinatyFormatter를 쓰면 편리하지만, gameobject hierarchy 같은 것을 쓰는  것은 불가능
            //또한 내가 모든 저장 형식에 대한 컨트롤을 가지는 편이 더 좋다(속도, 메모리 등)

            writer.Write(objects.Count);
            for(int i=0;i<objects.Count;i++)
            {
                Transform t = objects[i];
                writer.Write(t.localPosition.x);
                writer.Write(t.localPosition.y);
                writer.Write(t.localPosition.z);
            }
        }
    }

    void Load()
    {
        //데이터 있는 상태에서 load되면 안됨!
        BeginNewGame();
        using (var reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
        {
            int Count = reader.ReadInt32();
            for(int i=0;i<Count;i++)
            {
                Vector3 p;
                p.x = reader.ReadSingle();
                p.y = reader.ReadSingle();
                p.z = reader.ReadSingle();

                Transform t = Instantiate(prefab);
                t.localPosition = p;
                objects.Add(t);
            }
        }
    }

}
