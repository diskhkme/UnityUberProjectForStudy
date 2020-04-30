using System.IO;
using UnityEngine;

//High-level data reader wrapper class
public class GameDataReader
{
    //버전은 reader에서 관리하도록 변경
    public int Version { get; }

    BinaryReader reader;

    public GameDataReader(BinaryReader reader, int version)
    {
        this.reader = reader;
        this.Version = version;
    }

    public Random.State ReadRandomState()
    {
        return JsonUtility.FromJson<Random.State>(reader.ReadString());
    }

    public float ReadFloat()
    {
        return reader.ReadSingle();
    }
    public int ReadInt()
    {
        return reader.ReadInt32();
    }

    public Quaternion ReadQuaternion()
    {
        Quaternion value;
        value.x = reader.ReadSingle();
        value.y = reader.ReadSingle();
        value.z = reader.ReadSingle();
        value.w = reader.ReadSingle();
        return value;
    }
    public Vector3 ReadVector3()
    {
        Vector3 value;
        value.x = reader.ReadSingle();
        value.y = reader.ReadSingle();
        value.z = reader.ReadSingle();
        return value;
    }

    public Color ReadColor()
    {
        Color color;
        color.r = reader.ReadSingle();
        color.g = reader.ReadSingle();
        color.b = reader.ReadSingle();
        color.a = reader.ReadSingle();
        return color;

    }
}
