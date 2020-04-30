//hsv range를 slider로 사용하고 싶은데, 타입이 우리가 정의한 floatrage이므로, 유니티에서 제공하는 [Range(0,1)] Attribute는 사용 못함. 따로 만들어 주어야 함
using UnityEngine;

public class FloatRangeSliderAttribute : PropertyAttribute //propertyattribute를 상속 받아서 만들어야 함
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    public FloatRangeSliderAttribute(float min, float max)
    {
        if(max < min)
        {
            max = min;
        }
        Min = min;
        Max = max;
    }
    
}