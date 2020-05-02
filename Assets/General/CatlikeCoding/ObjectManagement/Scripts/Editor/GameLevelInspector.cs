using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameLevel))] //gamelevel component에 대한 custom inspectior
public class GameLevelInspector : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //default 표시는 그대로 사용

        var gameLevel = (GameLevel)target; //target은 현재 수정되고 있는 컴포넌트를 가져옴.
        if (gameLevel.HasMissingLevelObjects)
        {
            EditorGUILayout.HelpBox("Missing level objects!", MessageType.Error);
            if(GUILayout.Button("Remove Missing Elements"))
            {
                Undo.RecordObject(gameLevel, "Remove Missing Level Objects."); //undo가 가능하게 하려면 추가
                gameLevel.RemoveMissingLevelObjects();
            }
        }
    }
}