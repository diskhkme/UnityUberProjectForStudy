using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); //property들 그림

        MapGenerator map = target as MapGenerator;

        map.GenerateMap();
    }
}
