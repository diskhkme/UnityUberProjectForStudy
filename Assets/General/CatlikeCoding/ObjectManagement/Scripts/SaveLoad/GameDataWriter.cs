using System.IO;
using UnityEngine;

//High-level data writer wrapper class
public class GameDataWriter
{
    BinaryWriter writer;

    public GameDataWriter(BinaryWriter writer)
    {
        this.writer = writer;
    }

    public void Write(float value)
    {
        writer.Write(value);
    }
    public void Write(int value)
    {
        writer.Write(value);
    }

    public void Write(Quaternion value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
        writer.Write(value.w);
    }
    public void Write(Vector3 value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
    }

    public void Write(Color color)
    {
        writer.Write(color.r);
        writer.Write(color.g);
        writer.Write(color.b);
        writer.Write(color.a);
    }

    //Random 시퀀스를 이어나가고 싶다고 가정하고, 그 상태를 저장
    public void Write(Random.State value)
    {
        //Random 시퀀스는 4개의 floating point 데이터지만, 접근은 불가.
        //JSON serialization을 사용해 저장
        writer.Write(JsonUtility.ToJson(value));
    }

    public void Write(ShapeInstance value)
    {
        writer.Write(value.IsValid ? value.Shape.SaveIndex : -1); //invalid인 shape은 음수 index로
    }
}
