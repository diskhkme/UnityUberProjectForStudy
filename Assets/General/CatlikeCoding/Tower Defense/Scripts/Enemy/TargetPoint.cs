using UnityEngine;

namespace Defense
{
    //적 prefab의 targeting을 돕기 위한 compoenent. 현재 hierarchy에서는 model을 translate하여 회전 등을 수행하기 때문에 복잡함
    public class TargetPoint : MonoBehaviour
    {
        public Enemy Enemy { get; private set; }

        public Vector3 Position => transform.position;

        private void Awake()
        {
            Enemy = transform.root.GetComponent<Enemy>();
            Debug.Assert(Enemy != null, "Target point without enemy root!", this);
            Debug.Assert(GetComponent<SphereCollider>() != null, "Target point without sphere collider!", this);
            Debug.Assert(gameObject.layer == 10, "Target point on wrong layer!", this);
        }
    }
}
