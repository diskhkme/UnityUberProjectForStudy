﻿using UnityEngine;

//lifecycle 전체를 관리하기위한 behavior를 따로 정의
public sealed class LifecycleShapeBehavior : ShapeBehavior
{
    float adultDuration, dyingDuration, dyingAge;

    public override ShapeBehaviorType BehaviorType
    {
        get
        {
            return ShapeBehaviorType.Lifecycle;
        }
    }

    public void Initialize(Shape shape, float growingDuration, float adultDuration, float dyingDuration)
    {
        this.adultDuration = adultDuration;
        this.dyingDuration = dyingDuration;
        dyingAge = growingDuration + adultDuration;

        if(growingDuration > 0f)
        {
            shape.AddBehavior<GrowingShapeBehavior>().Initialize(shape, growingDuration);
        }
    }

    public override bool GameUpdate(Shape shape)
    {
        if(shape.Age >= dyingAge)
        {
            if(dyingDuration <= 0f)
            {
                shape.Die();
                return true;
            }
            if(!shape.IsMarkedAsDying)
            {
                shape.AddBehavior<DyingShapeBehavior>().Initialize(shape, dyingDuration + dyingAge - shape.Age);
            }
            return false;
        }
        return true;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(adultDuration);
        writer.Write(dyingDuration);
        writer.Write(dyingAge);
    }

    public override void Load(GameDataReader reader)
    {
        adultDuration = reader.ReadFloat();
        dyingDuration = reader.ReadFloat();
        dyingAge = reader.ReadFloat();
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<LifecycleShapeBehavior>.Reclaim(this);
    }
}