[System.Serializable]
public struct ShapeInstance
{
    public Shape Shape { get; private set; }

    int instanceId;

    public ShapeInstance(Shape shape)
    {
        Shape = shape;
        instanceId = shape.InstanceId;
    }

    public bool IsValid
    {
        get
        {
            return Shape && instanceId == Shape.InstanceId;
        }
    }

    //ShaepInstance casting overloading
    public static implicit operator ShapeInstance(Shape shape)
    {
        return new ShapeInstance(shape); // ShapeInstance s = new Shape(); 가능해짐
    }

}