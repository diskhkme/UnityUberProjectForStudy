using UnityEngine;

[System.Serializable] //inspector에 보이도록
public struct IntRange
{
    public int min, max;

    public int RandomValueInRange
    {
        get
        {
            return Random.Range(min, max+1);
        }
    }
}
