using UnityEngine;

[System.Serializable] //inspector에 보이도록
public struct FloatRange
{
    public float min, max;

    public float RandomValueInRange
    {
        get
        {
            return Random.Range(min, max);
        }
    }
}
