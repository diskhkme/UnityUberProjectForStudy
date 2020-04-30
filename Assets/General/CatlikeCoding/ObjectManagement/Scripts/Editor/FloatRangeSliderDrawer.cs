//FloatRange를 좀 더 잘보여using UnityEditor;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FloatRangeSliderAttribute))] //어떤 타입에 대한 property drawer인지 알려주어야 함
public class FloatRangeSliderDrawer : PropertyDrawer
{
    //OnGUI 오버라이드 해서 구현
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int originalIndentLevel = EditorGUI.indentLevel;
        EditorGUI.BeginProperty(position, label, property);
        //begin과 end 사이에 작업할 코드를 작성
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        EditorGUI.indentLevel = 0;

        SerializedProperty minProperty = property.FindPropertyRelative("min");
        SerializedProperty maxProperty = property.FindPropertyRelative("max");
        float minValue = minProperty.floatValue;
        float maxValue = maxProperty.floatValue;

        float fieldWidth = position.width / 4f - 4f;
        float sliderWidth = position.width / 2f;
        position.width = fieldWidth;
        minValue = EditorGUI.FloatField(position, minValue);
        position.x += fieldWidth + 4f;
        position.width = sliderWidth;

        FloatRangeSliderAttribute limit = attribute as FloatRangeSliderAttribute;
        EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, limit.Min, limit.Max);

        position.x += position.width + 4f;
        position.width = fieldWidth;
        maxValue = EditorGUI.FloatField(position, maxValue);

        if(minValue < limit.Min)
            minValue = limit.Min;
        else if(minValue > limit.Max)
            minValue = limit.Max;
        if (maxValue < minValue)
            maxValue = minValue;
        else if (maxValue > limit.Max)
            maxValue = limit.Max;
        

        minProperty.floatValue = minValue;
        maxProperty.floatValue = maxValue;
        EditorGUI.EndProperty();
        EditorGUI.indentLevel = originalIndentLevel;

    }
}