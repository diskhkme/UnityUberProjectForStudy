using UnityEngine;

public sealed class SatelliteShapeBehavior : ShapeBehavior
{
    Shape focalShape;
    float frequency;
    Vector3 cosOffset, sinOffset;

    public override ShapeBehaviorType BehaviorType
    {
        get
        {
            return ShapeBehaviorType.Satellite;
        }
    }

    //spawn zone에서 motion을 생성해 저정의하는 것은 적절하지 않으므로 여기에 initialize method 정의
    public void Initialize(Shape shape, Shape focalShape, float radius, float frequency)
    {
        this.focalShape = focalShape;
        this.frequency = frequency;
        Vector3 orbitAxis = Random.onUnitSphere;
        do
        {
            cosOffset = Vector3.Cross(orbitAxis, Random.onUnitSphere).normalized;
        } while (cosOffset.sqrMagnitude < 0.1f);
        sinOffset = Vector3.Cross(cosOffset, orbitAxis);
        
        cosOffset *= radius;
        sinOffset *= radius;

        //focal shape을 항상 바라보도록 rotation
        shape.AddBehavior<RotationShapeBehavior>().AngularVelocity = -360f * frequency * shape.transform.InverseTransformDirection(orbitAxis);

        GameUpdate(shape); //초기 위치를 맞추기 위해 GameUpdate 한번 호출
    }

    public override void GameUpdate(Shape shape)
    {
        float t = 2f * Mathf.PI * frequency * shape.Age;
        shape.transform.localPosition = focalShape.transform.localPosition + cosOffset * Mathf.Cos(t) + sinOffset * Mathf.Sin(t);
    }

    public override void Save(GameDataWriter writer) { }

    public override void Load(GameDataReader reader) { }

    public override void Recycle()
    {
        ShapeBehaviorPool<SatelliteShapeBehavior>.Reclaim(this);
    }
}