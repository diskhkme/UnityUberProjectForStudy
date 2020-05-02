using UnityEngine;

//상태를 저장해야하므로 PersistableObject로
public partial class GameLevel : PersistableObject
{
    public static GameLevel Current { get; private set; }

    [SerializeField] SpawnZone spawnZone;

    [UnityEngine.Serialization.FormerlySerializedAs("persistentObjects")] //이미 serialize된 것을 이름을 바꿔 옮기고 싶을때 사용하는 attribute
    [SerializeField] GameLevelObject[] levelObjects;

    [SerializeField] int populationLimit;
    public int PopulationLimit
    {
        get
        {
            return populationLimit;
        }
    }
    
    private void OnEnable()
    {
        Current = this;
        if(levelObjects == null)
        {
            levelObjects = new GameLevelObject[0];
        }
    }

    public void GameUpdate()
    {
        for(int i=0;i<levelObjects.Length;i++)
        {
            levelObjects[i].GameUpdate();
        }
    }

    public void SpawnShape()
    {
        spawnZone.SpawnShape();
    }



    public override void Save(GameDataWriter writer)
    {
        writer.Write(levelObjects.Length);
        for(int i=0;i< levelObjects.Length;i++)
        {
            levelObjects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int savedCount = reader.ReadInt();
        for(int i=0;i<savedCount;i++)
        {
            levelObjects[i].Load(reader);
        }
    }
}
