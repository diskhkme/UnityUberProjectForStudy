using UnityEngine;

public class CubeSpawnZone : SpawnZone
{
    [SerializeField] bool surfaceOnly;

    public override Vector3 SpawnPoint
    {
        get
        {
            Vector3 p;
            p.x = Random.Range(-0.5f, 0.5f);
            p.y = Random.Range(-0.5f, 0.5f);
            p.z = Random.Range(-0.5f, 0.5f);
            if(surfaceOnly)
            {
                int axis = Random.Range(0, 3);
                p[axis] = p[axis] < 0f ? -0.5f : 0.5f; //세 축중에 하나를 -0.5 or 0.5로
            }
            return transform.TransformPoint(p);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = this.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }

}
