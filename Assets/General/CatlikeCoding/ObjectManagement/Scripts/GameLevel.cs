using UnityEngine;

//game level에 대한 제어
public class GameLevel : MonoBehaviour
{
    [SerializeField] SpawnZone spawnZone;

    private void Start()
    {
        Game.Instance.spawnZoneOfLevel = spawnZone;
    }
}
