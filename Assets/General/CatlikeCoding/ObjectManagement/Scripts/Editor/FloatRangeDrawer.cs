//FloatRange를 좀 더 잘보여주기위한 에디터 클래스.
//에디터 클래스는 Editor 폴더 내에 있어야 유니티에서 따로 처리하여 빌드에서는 제거함.

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FloatRange)), CustomPropertyDrawer(typeof(IntRange))]
public class FloatOrIntRangeDrawer : PropertyDrawer
{
    //OnGUI 오버라이드 해서 구현
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int originalIndentLevel = EditorGUI.indentLevel;
        float originalLabelWidth = EditorGUIUtility.labelWidth;

        EditorGUI.BeginProperty(position, label, property);
        //begin과 end 사이에 작업할 코드를 작성
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive),label); //왼쪽에 표시할 label;
        position.width = position.width / 2f; //좌우로 나란히 minmax를 표시하기 위해 절반 크기씩 차지하도록
        EditorGUIUtility.labelWidth = position.width / 2f; //label이 너무 크므로 크기를 줄임
        EditorGUI.indentLevel = 1;

        EditorGUI.PropertyField(position, property.FindPropertyRelative("min")); //min 이라는 필드 값을 propertyfield로 나타냄
        position.x += position.width; //max값은 오른쪽에
        EditorGUI.PropertyField(position, property.FindPropertyRelative("max"));

        EditorGUI.EndProperty();

        EditorGUI.indentLevel = originalIndentLevel;
        EditorGUIUtility.labelWidth = originalLabelWidth;
    }
}