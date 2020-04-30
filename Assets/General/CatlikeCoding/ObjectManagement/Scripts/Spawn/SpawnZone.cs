using UnityEngine;

public abstract class SpawnZone : MonoBehaviour
{
    public abstract Vector3 SpawnPoint { get; }
    //[SerializeField] bool surfaceOnly;


    //public Vector3 SpawnPoint
    //{
    //    get
    //    {
    //        //return Random.insideUnitSphere * 5f + this.transform.position;
    //        return this.transform.TransformPoint(surfaceOnly ? Random.onUnitSphere : Random.insideUnitSphere); //local position & rotation & scale 적용을 위해.
    //    }
    //}

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.cyan;
    //    Gizmos.matrix = this.transform.localToWorldMatrix; //default는 world에서 그려지기 때문에, local transform 적용하려면 matrix 바꿔주면 됨.
    //    Gizmos.DrawWireSphere(Vector3.zero, 1f);
    //}
}
