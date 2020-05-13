using UnityEngine;

namespace Defense
{
    public enum TowerType
    {
        Laser, Mortar
    }

    public abstract class Tower : GameTileContent
    {
        static Collider[] targetsBuffer = new Collider[100]; //overlab에서 반복적 allocation을 방지
        const int enemyLayerMask = 1 << 10;
        
        [SerializeField, Range(1.5f, 10.5f)] float targetingRange = 1.5f;

        public abstract TowerType TowerType { get; }

        void OnDrawGizmosSelected() //tower의 range을 보여주되, 선택된 객체에 대해서만 보여줌
        {
            Gizmos.color = Color.yellow;
            Vector3 position = transform.localPosition;
            position.y += 0.01f;
            Gizmos.DrawWireSphere(position, targetingRange);
        }

        protected bool AcquireTarget(out TargetPoint target)
        {
            Vector3 a = transform.localPosition;
            Vector3 b = a;
            b.y += 3f;
            int hits = Physics.OverlapCapsuleNonAlloc(a, b, targetingRange, targetsBuffer, enemyLayerMask);
            //높이 때문에 capsule overlap test로 변경, 새 배열을 할당 안하는 nonalloc overlab test method 사용

            if (hits > 0)
            {
                target = targetsBuffer[Random.Range(0, hits)].GetComponent<TargetPoint>();
                Debug.Assert(target != null, "Targeted non-enemy!", targetsBuffer[0]);
                return true;
            }
            target = null;
            return false;
        }

        protected bool TrackTarget(ref TargetPoint target) 
        {
            if (target == null)
            {
                return false;
            }
            Vector3 a = transform.localPosition;
            Vector3 b = target.Position;
            float x = a.x - b.x; //높이 차이 무시하고 계산
            float z = a.z - b.z;
            float r = targetingRange + 0.125f * target.Enemy.Scale;
            if (x * x + z * z > r * r) //enemy sphere collider의 radius
            {
                target = null;
                return false;
            }

            return true;
        }
    }
}
