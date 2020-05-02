[System.Serializable]
public struct ShapeInstance
{
    public Shape Shape { get; private set; }

    //satellite 또는 focus shape중 어떤것이 먼저 로딩될지 모르므로 둘다 로딩될때까지 postpone 필요
    int instanceIdOrSaveIndex;

    public ShapeInstance(Shape shape)
    {
        Shape = shape;
        instanceIdOrSaveIndex = shape.InstanceId;
    }

    public ShapeInstance(int saveIndex)
    {
        Shape = null;
        instanceIdOrSaveIndex = saveIndex;
    }

    public void Resolve()
    {
        if(instanceIdOrSaveIndex >= 0)
        {
            Shape = Game.Instance.GetShape(instanceIdOrSaveIndex);
            instanceIdOrSaveIndex = Shape.InstanceId;
        }
        
    }

    public bool IsValid
    {
        get
        {
            return Shape && instanceIdOrSaveIndex == Shape.InstanceId;
        }
    }

    //ShaepInstance casting overloading
    public static implicit operator ShapeInstance(Shape shape)
    {
        return new ShapeInstance(shape); // ShapeInstance s = new Shape(); 가능해짐
    }

}