using UnityEngine;

public abstract class SpawnZone : PersistableObject
{
    public abstract Vector3 SpawnPoint { get; }

    //Spawn zone별로 다른 velocity를 주기 위함.
    public virtual void ConfigureSpawn(Shape shape)
    {
        Transform t = shape.transform;
        t.localPosition = SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        shape.SetColor(Random.ColorHSV(hueMin: 0f, hueMax: 1f,
                                           saturationMin: 0.5f, saturationMax: 1f,
                                           valueMin: 0.25f, valueMax: 1f,
                                           alphaMin: 1f, alphaMax: 1f));
        shape.AngularVelocity = Random.onUnitSphere * Random.Range(0f, 90f); //shape rotation
        shape.Velocity = this.transform.forward * Random.Range(0f, 2f); //velocity 경향성을 주도록 변경
    }
    
}
