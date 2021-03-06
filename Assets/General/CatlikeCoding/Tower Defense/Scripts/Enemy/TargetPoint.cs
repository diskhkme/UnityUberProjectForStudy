﻿using UnityEngine;

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

        //---적 정보를 가져오기 위한 static method들---
        const int enemyLayerMask = 1 << 10;

        static Collider[] buffer = new Collider[100];

        public static int BufferedCount { get; private set; }
        public static TargetPoint RandomBuffered => GetBuffered(Random.Range(0, BufferedCount));

        public static bool FillBuffer(Vector3 position, float range)
        {
            Vector3 top = position;
            top.y += 3f;
            BufferedCount = Physics.OverlapCapsuleNonAlloc(
                position, top, range, buffer, enemyLayerMask
            );
            return BufferedCount > 0;
        }

        public static TargetPoint GetBuffered(int index)
        {
            var target = buffer[index].GetComponent<TargetPoint>();
            Debug.Assert(target != null, "Targeted non-enemy!", buffer[0]);
            return target;
        }
    }
}
