using UnityEngine;

//상태를 저장해야하므로 PersistableObject로
public class GameLevel : PersistableObject
{
    //이제는 게임 도중에 일어난 상태까지 저장할 수 있도록, game state를 만드는 것이 필요
    public static GameLevel Current { get; private set; }

    [SerializeField] SpawnZone spawnZone;

    //상태를 저장해야 하는 persistable object들을 game level에서 관리
    [SerializeField] PersistableObject[] persistentObjects; 

    private void OnEnable()
    {
        Current = this;
        if(persistentObjects == null)
        {
            persistentObjects = new PersistableObject[0];
        }
    }

    public Shape SpawnShape()
    {
        return spawnZone.SpawnShape();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(persistentObjects.Length);
        for(int i=0;i<persistentObjects.Length;i++)
        {
            persistentObjects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int savedCount = reader.ReadInt();
        for(int i=0;i<savedCount;i++)
        {
            persistentObjects[i].Load(reader);
        }
    }
}
