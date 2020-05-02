using UnityEngine;

public sealed class DyingShapeBehavior : ShapeBehavior
{
    Vector3 originalScale;
    float duration, dyingAge;

    public override ShapeBehaviorType BehaviorType
    {
        get
        {
            return ShapeBehaviorType.Dying;
        }
    }

    public void Initialize(Shape shape, float duration)
    {
        originalScale = shape.transform.localScale;
        this.duration = duration;
        dyingAge = shape.Age; //죽기 시작하는 age
    }

    public override bool GameUpdate(Shape shape)
    {
        float dyingDuration = shape.Age - dyingAge;

        if (dyingDuration < duration)
        {
            float s = 1f - dyingDuration / duration;
            s = (3f - 2f * s) * s * s; //damped grawing
            shape.transform.localScale = s * originalScale;
            return true;
        }
        //shape.transform.localScale = Vector3.zero;
        shape.Die(); //축소가 다 된 shape은 제거
        return true; //recycle까지 했기 때문에 (die 호출하면서) true 반환
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(originalScale);
        writer.Write(duration);
        writer.Write(dyingAge);
    }

    public override void Load(GameDataReader reader)
    {
        originalScale = reader.ReadVector3();
        duration = reader.ReadFloat();
        dyingAge = reader.ReadFloat();
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<DyingShapeBehavior>.Reclaim(this);
    }
}