using UnityEngine;

public abstract class SpawnZone : GameLevelObject
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
        public MovementDirection oscillationDirection;
        public FloatRange oscillationAmplitude;
        public FloatRange oscilllationFrequency;

        [System.Serializable]
        public struct SatelliteConfiguration
        {
            public IntRange amount;
            [FloatRangeSlider(0.1f, 1f)]
            public FloatRange relativeScale;
            public FloatRange orbitRadius;
            public FloatRange orbitFrequency;
            public bool uniformLifecycles;
        }

        public SatelliteConfiguration satellite;
       
        [System.Serializable]
        public struct LifecycleConfiguration
        {
            [FloatRangeSlider(0f, 2f)]
            public FloatRange growingDuration;
            [FloatRangeSlider(0f, 100f)]
            public FloatRange adultDuration;
            [FloatRangeSlider(0f, 2f)]
            public FloatRange dyingDuration;

            public Vector3 RandomDurations
            {
                get
                {
                    return new Vector3(growingDuration.RandomValueInRange,
                                        adultDuration.RandomValueInRange,
                                        dyingDuration.RandomValueInRange);
                }
            }
        }

        public LifecycleConfiguration lifecycle;

        public ColorRangeHSV color;
        public bool uniformColor;
    }

    [SerializeField] SpawnConfiguration spawnConfig;

    public abstract Vector3 SpawnPoint { get; }

    [SerializeField, Range(0f, 50f)] float spawnSpeed;
    float spawnProgress;

    public override void GameUpdate()
    {
        spawnProgress += Time.deltaTime * spawnSpeed;
        while (spawnProgress >= 1f)
        {
            spawnProgress -= 1f;
            SpawnShape();
        }
    }
    
    //이제 shape의 spawn을 game에서 zone의 역할로 가져옴
    public virtual void SpawnShape()
    {
        int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
        Shape shape = spawnConfig.factories[factoryIndex].GetRandom();
        shape.gameObject.layer = gameObject.layer;  //특정 레이어의 존에서 생성된 shape은 그 레이어를 가짐. 다른 레이어는 충돌검사 하지 않도록 editor에서 설정함
        Transform t = shape.transform;
        t.localPosition = SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;
        SetupColor(shape);

        float angularSpeed = spawnConfig.angularSpeed.RandomValueInRange;
        if (angularSpeed != 0f) //필요한 경우에만 컴포넌트로 추가
        {
            //var rotation = shape.gameObject.AddComponent<RotationShapeBehavior>();
            var rotation = shape.AddBehavior<RotationShapeBehavior>();
            rotation.AngularVelocity = Random.onUnitSphere * angularSpeed;
        }

        float speed = spawnConfig.speed.RandomValueInRange;
        if (speed != 0f)
        {
            var movement = shape.AddBehavior<MovementShapeBehavior>();
            movement.Velocity = GetDirectionVector(spawnConfig.movementDirection, t) * speed;
        }

        SetupOscillation(shape);

        Vector3 lifecycleDuration = spawnConfig.lifecycle.RandomDurations;

        int satelliteCount = spawnConfig.satellite.amount.RandomValueInRange;
        for(int i=0;i<satelliteCount;i++)
        {
            CreateSatelliteFor(shape, 
                spawnConfig.satellite.uniformLifecycles ? lifecycleDuration : spawnConfig.lifecycle.RandomDurations);
        }

        SetupLifecycle(shape, lifecycleDuration);
        
    }

    //특정 shape에 딸려 생기는 위성 shape 생성
    void CreateSatelliteFor(Shape focalShape, Vector3 lifecycleDuration)
    {
        int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
        Shape shape = spawnConfig.factories[factoryIndex].GetRandom();
        shape.gameObject.layer = gameObject.layer;
        Transform t = shape.transform;
        t.localRotation = Random.rotation;
        t.localScale = focalShape.transform.localScale * spawnConfig.satellite.relativeScale.RandomValueInRange;
        t.localPosition = focalShape.transform.localPosition + Vector3.up;
        shape.AddBehavior<MovementShapeBehavior>().Velocity = Vector3.up;
        SetupColor(shape);
        //return을 해야 game에 가서 update loop를 돌게 되는데, 아직 없음. 확장 필요. Game 에 instance 생성
        shape.AddBehavior<SatelliteShapeBehavior>().Initialize(shape, focalShape,
            spawnConfig.satellite.orbitRadius.RandomValueInRange,
            spawnConfig.satellite.orbitFrequency.RandomValueInRange);
        SetupLifecycle(shape, lifecycleDuration);
    }

    private void SetupColor(Shape shape)
    {
        if (spawnConfig.uniformColor)
        {
            shape.SetColor(spawnConfig.color.RandomInRange);
        }
        else
        {
            for (int i = 0; i < shape.ColorCount; i++)
            {
                shape.SetColor(spawnConfig.color.RandomInRange, i);
            }
        }
    }
       
    void SetupOscillation(Shape shape)
    {
        float amplitude = spawnConfig.oscillationAmplitude.RandomValueInRange;
        float frequency = spawnConfig.oscilllationFrequency.RandomValueInRange;
        if(amplitude == 0f || frequency == 0f)
        {
            return;
        }
        var oscillation = shape.AddBehavior<OscillationShapeBehavior>();
        oscillation.Offset = GetDirectionVector(spawnConfig.oscillationDirection, shape.transform) * amplitude;
        oscillation.Frequency = frequency;
    }

    Vector3 GetDirectionVector(SpawnConfiguration.MovementDirection direction, Transform t)
    {
        switch(direction)
        {
            case SpawnConfiguration.MovementDirection.Upward:
                return transform.up;
            case SpawnConfiguration.MovementDirection.Outward:
                return (t.localPosition - transform.position).normalized;
            case SpawnConfiguration.MovementDirection.Random:
                return Random.onUnitSphere;
            default:
                return transform.forward;
        }
    }
    
    void SetupLifecycle (Shape shape, Vector3 durations)
    {
        if (durations.x > 0f) //growing이 있고,
        {
            if (durations.y > 0f || durations.z > 0f) //adult와 dying 둘 중 하나라도 있으면
            {
                shape.AddBehavior<LifecycleShapeBehavior>().Initialize( //1)모두 다 커버하는 lifecycle 추가
                    shape, durations.x, durations.y, durations.z
                );
            }
            else //아니면 growing만 필요
            {
                shape.AddBehavior<GrowingShapeBehavior>().Initialize(
                    shape, durations.x
                );
            }
        }
        else if (durations.y > 0f)
        {
            shape.AddBehavior<LifecycleShapeBehavior>().Initialize(
                shape, durations.x, durations.y, durations.z
            );
        }
        else if (durations.z > 0f)
        {
            shape.AddBehavior<DyingShapeBehavior>().Initialize(
                shape, durations.z
            );
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(spawnProgress);
    }

    public override void Load(GameDataReader reader)
    {
        spawnProgress = reader.ReadFloat();
    }
}
