using UnityEngine;

//상태를 저장해야하므로 PersistableObject로
public class GameLevel : PersistableObject
{
    //이제는 게임 도중에 일어난 상태까지 저장할 수 있도록, game state를 만드는 것이 필요
    public static GameLevel Current { get; private set; }

    [SerializeField] SpawnZone spawnZone;

    private void OnEnable()
    {
        Current = this;
    }

    public Vector3 SpawnPoint //spawnPoint 자체를 game level이 대리
    {
        get
        {
            return spawnZone.SpawnPoint;
        }
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
    }
}
