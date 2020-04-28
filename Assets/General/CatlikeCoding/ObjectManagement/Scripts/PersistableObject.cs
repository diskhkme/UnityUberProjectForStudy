using UnityEngine;

//같은 컴포넌트를 여러개 갖지 못하도록 제한하는 attribute
[DisallowMultipleComponent]
public class PersistableObject : MonoBehaviour
{
    public void Save(GameDataWriter writer)
    {
        writer.Write(transform.localPosition);
        writer.Write(transform.localRotation);
        writer.Write(transform.localScale);
    }

    public void Load(GameDataReader reader)
    {
        transform.localPosition = reader.ReadVector3();
        transform.localRotation = reader.ReadQuaternion();
        transform.localScale = reader.ReadVector3();
    }
}