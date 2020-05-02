public enum ShapeBehaviorType
{
    Movement,
    Rotation,
    Oscillation,
    Satellite,
    Growing
}

//shape에서 하던 type의 정의를 이리로 가져옴
public static class ShapeBehaviorTypeMethods 
{
    public static ShapeBehavior GetInstance(this ShapeBehaviorType type)//extension method
    {
        switch (type)
        {
            case ShapeBehaviorType.Movement:
                return ShapeBehaviorPool<MovementShapeBehavior>.Get();
            case ShapeBehaviorType.Rotation:
                return ShapeBehaviorPool<RotationShapeBehavior>.Get();
            case ShapeBehaviorType.Oscillation:
                return ShapeBehaviorPool<OscillationShapeBehavior>.Get();
            case ShapeBehaviorType.Satellite:
                return ShapeBehaviorPool<SatelliteShapeBehavior>.Get();
            case ShapeBehaviorType.Growing:
                return ShapeBehaviorPool<GrowingShapeBehavior>.Get();
        }
        UnityEngine.Debug.Log("Forgot to support" + type);
        return null;
    }
}