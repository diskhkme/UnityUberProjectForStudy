using UnityEngine;

public sealed class RotationShapeBehavior : ShapeBehavior
{
    public Vector3 AngularVelocity { get; set; }
    public override ShapeBehaviorType BehaviorType
    {
        get
        {
            return ShapeBehaviorType.Movement;
        }
    }

    public override void GameUpdate(Shape shape)
    {
        shape.transform.Rotate(AngularVelocity * Time.deltaTime);
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<RotationShapeBehavior>.Reclaim(this);
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(AngularVelocity);
    }

    public override void Load(GameDataReader reader)
    {
        AngularVelocity = reader.ReadVector3();
    }

}
