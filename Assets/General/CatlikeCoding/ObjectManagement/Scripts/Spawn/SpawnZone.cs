using UnityEngine;

public abstract class SpawnZone : PersistableObject
{
    [System.Serializable]
    public struct SpawnConfiguration
    {
        public enum MovementDirection
        {
            Forward, Upward, Outward, Random
        }
        //zone마다 factory를 선택 가능하도록
        public ShapeFactory[] factories;
        public MovementDirection movementDirection;
        public FloatRange speed;
        public FloatRange angularSpeed;
        public FloatRange scale;
        public ColorRangeHSV color;
        public bool uniformColor;
    }

    [SerializeField] SpawnConfiguration spawnConfig;

    public abstract Vector3 SpawnPoint { get; }
    

    //이제 shape의 spawn을 game에서 zone의 역할로 가져옴
    public virtual Shape SpawnShape()
    {
        int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
        Shape shape = spawnConfig.factories[factoryIndex].GetRandom();
        Transform t = shape.transform;
        t.localPosition = SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;
        if(spawnConfig.uniformColor)
        {
            shape.SetColor(spawnConfig.color.RandomInRange);
        }
        else
        {
            for(int i=0;i<shape.ColorCount;i++)
            {
                shape.SetColor(spawnConfig.color.RandomInRange, i);
            }
        }
        
        shape.AngularVelocity = Random.onUnitSphere * spawnConfig.angularSpeed.RandomValueInRange;

        Vector3 direction;
        switch(spawnConfig.movementDirection)
        {
            case SpawnConfiguration.MovementDirection.Upward:
                direction = this.transform.up;
                break;
            case SpawnConfiguration.MovementDirection.Outward:
                direction = (t.localPosition - this.transform.position).normalized;
                break;
            case SpawnConfiguration.MovementDirection.Random:
                direction = Random.onUnitSphere;
                break;
            default:
                direction = this.transform.forward;
                break;
        }
        
        shape.Velocity = direction * spawnConfig.speed.RandomValueInRange; //velocity 경향성을 주도록 변경

        return shape;
    }
    
}
