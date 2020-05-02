using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] float dyingDuration;

    private void OnTriggerEnter(Collider other)
    {
        var shape = other.GetComponent<Shape>();
        if(shape)
        {
            if(dyingDuration <= 0f)
            {
                shape.Die();
            }
            else if(!shape.IsMarkedAsDying)
            {
                shape.AddBehavior<DyingShapeBehavior>().Initialize(shape, dyingDuration);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        //Gizmos.matrix = transform.localToWorldMatrix; //이렇게 하면 nonuniform sclae이 제대로 표시 안됨.(우리가 설정한 scale대로 표시되지만, unity physics에서는 이걸 계산에 사용하지 않고 uniform scale을 사용함)
        var c = GetComponent<Collider>();
        var b = c as BoxCollider;
        if (b != null)
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(b.center, b.size);
            return;
        }
        var s = c as SphereCollider;
        if (s != null)
        {
            Vector3 scale = transform.lossyScale;
            scale = Vector3.one * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, scale);
            Gizmos.DrawWireSphere(s.center, s.radius);
            return;
        }
    }
}
