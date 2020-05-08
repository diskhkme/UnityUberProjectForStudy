using UnityEngine;

namespace Defense
{
    //이전과 마찬가지로 factory를 scriptable object로 정의
    [CreateAssetMenu]
    public class GameTileContentFactory : GameObjectFactory
    {
        [SerializeField] GameTileContent destinationPrefab = default;
        [SerializeField] GameTileContent emptyPrefab = default;
        [SerializeField] GameTileContent wallPrefab = default;
        [SerializeField] GameTileContent spawnPointPrefab = default;

        public void Reclaim(GameTileContent content)
        {
            Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed!");
            Destroy(content.gameObject);
        }

        GameTileContent Get(GameTileContent prefab)
        {
            GameTileContent instance = CreateGameObjectInstance(prefab); //이제는 base class에서 생성 및 이동을 담당
            instance.OriginFactory = this;
            return instance;
        }

        //public Get(). 요청하는 타입의 컨텐츠를 생성하고 생성된 것을 반환
        public GameTileContent Get(GameTileContentType type)
        {
            switch (type)
            {
                case GameTileContentType.Destination: return Get(destinationPrefab);
                case GameTileContentType.Empty: return Get(emptyPrefab);
                case GameTileContentType.Wall: return Get(wallPrefab);
                case GameTileContentType.SpawnPoint: return Get(spawnPointPrefab);
            }
            Debug.Assert(false, "Unsupported type: " + type);
            return null;
        }

    }

}
