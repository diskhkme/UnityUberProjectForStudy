using UnityEngine;

namespace Defense
{
    //mortar shell의 폭발 효과 처리
    public class Explosion : WarEntity
    {
        static int colorPropertyID = Shader.PropertyToID("_Color");
        static MaterialPropertyBlock propertyBlock;

        [SerializeField, Range(0f, 1f)] float duration = 0.5f;
        [SerializeField] AnimationCurve opacityCurve = default;
        [SerializeField] AnimationCurve scaleCurve = default;

        float age;
        float scale;
        MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            Debug.Assert(meshRenderer != null, "Explosion without renderer!");
        }

        public void Initialize(Vector3 position, float blastRadius, float damage = 0f)
        {
            if(damage > 0f)
            {
                TargetPoint.FillBuffer(position, blastRadius);
                for (int i = 0; i < TargetPoint.BufferedCount; i++)
                {
                    TargetPoint.GetBuffered(i).Enemy.ApplyDamage(damage); //damage apply를 여기서 하도록 함
                }
            }
            
            transform.localPosition = position;
            scale = 2f * blastRadius;
        }

        public override bool GameUpdate()
        {
            age += Time.deltaTime;
            if (age >= duration)
            {
                OriginFactory.Reclaim(this);
                return false;
            }

            if(propertyBlock == null)
            {
                propertyBlock = new MaterialPropertyBlock();
            }
            float t = age / duration;
            Color c = Color.clear;
            c.a = opacityCurve.Evaluate(t); //curve를 사용하고, evaluate method로 값을 샘플링 할 수 있음!
            propertyBlock.SetColor(colorPropertyID, c);
            meshRenderer.SetPropertyBlock(propertyBlock);
            transform.localScale = Vector3.one * (scale * scaleCurve.Evaluate(t));

            return true;
        }
    }
}
