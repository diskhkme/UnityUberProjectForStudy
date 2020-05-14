using UnityEngine;

//발사체 관리를 위한 Factory 클래스
namespace Defense
{
    [CreateAssetMenu]
    public class WarFactory : GameObjectFactory
    {
        [SerializeField] Explosion explosionPrefab = default;
        [SerializeField] Shell shellPrefab = default;

        public Explosion Explosion => Get(explosionPrefab);
        public Shell Shell => Get(shellPrefab);

        T Get<T>(T prefab) where T : WarEntity
        {
            T instance = CreateGameObjectInstance(prefab);
            instance.OriginFactory = this;
            return instance;
        }

        public void Reclaim(WarEntity entity)
        {
            Debug.Assert(entity.OriginFactory == this, "Wrong factory reclaimed!");
            Destroy(entity.gameObject);
        }
    }
}
