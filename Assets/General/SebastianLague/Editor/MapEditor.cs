using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI(); //property들 그림
        MapGenerator map = target as MapGenerator;
        if (DrawDefaultInspector()) //값이 바뀌었을 때
        {
            map.GenerateMap();
        }
        
        if(GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }
    }
}
