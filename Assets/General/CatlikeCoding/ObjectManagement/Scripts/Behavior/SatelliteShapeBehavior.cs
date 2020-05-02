using UnityEngine;

public sealed class SatelliteShapeBehavior : ShapeBehavior
{
    ShapeInstance focalShape;
    float frequency;
    Vector3 cosOffset, sinOffset;
    Vector3 previousPosition; //focus가 없어지면 움직이던 방향대로 날아가도록 구현하기 위해 추가

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
        previousPosition = shape.transform.localPosition; //focus가 없어지는 것이 첫 프레임일수도 있고, 이전 데이터가 남아있을 수도 있으므로, 초기화
    }

    public override bool GameUpdate(Shape shape)
    {
        if(focalShape.IsValid)
        {
            float t = 2f * Mathf.PI * frequency * shape.Age;
            previousPosition = shape.transform.localPosition;
            shape.transform.localPosition = focalShape.Shape.transform.localPosition + cosOffset * Mathf.Cos(t) + sinOffset * Mathf.Sin(t);
            return true;
        }
        shape.AddBehavior<MovementShapeBehavior>().Velocity = (shape.transform.localPosition - previousPosition) / Time.deltaTime;
        return false; //satellite의 경우 focal shape이 valid일때만 true
    }

    public override void Save(GameDataWriter writer) { }

    public override void Load(GameDataReader reader) { }

    public override void Recycle()
    {
        ShapeBehaviorPool<SatelliteShapeBehavior>.Reclaim(this);
    }
}