using UnityEditor;
using UnityEngine;

static class RegisterLevelObjectMenuItem
{
    const string menuItem = "GameObject/Register Level Object";

    [MenuItem(menuItem, true)]
    static bool ValidateRegisterLevelObject() //특정 상황일 때 menuItem을 valid 상태로 만들기 위함
    {
        if (Selection.objects.Length == 0) //선택이 된 것이 없으면 false
        {
            return false;
        }
        foreach (Object o in Selection.objects)
        {
            if (!(o is GameObject)) //게임오브젝트가 아닌 것이 있으면 false
            {
                return false;
            }
        }
        return true;
    }

    //level object는 현재 gamelevel에 추가하는 메뉴 아이템 생성
    [MenuItem(menuItem)]
    static void RegisterLevelObject()
    {
        //다중 선택 가능하도록 변경
        foreach(Object o in Selection.objects)
        {
            Register(o as GameObject);
        }
    }


    static void Register(GameObject o)
    {
        //GameObject o = Selection.activeGameObject; //현재 선택되어있는 게임 오브젝트 가져오기
        if (PrefabUtility.GetPrefabAssetType(o) != PrefabAssetType.NotAPrefab) //선택된 것이 scene의 게임오브젝트거나, prefab일 수 있음. prefab이면 안되므로 예외 처리
        {
            Debug.LogWarning(o.name + " is a prefab asset.", o);
            return;
        }

        var levelObject = o.GetComponent<GameLevelObject>();
        if (levelObject == null)
        {
            Debug.LogWarning(o.name + " isn't a game level object.", o);
            return;
        }

        foreach (GameObject rootObject in o.scene.GetRootGameObjects()) //scene의 root에서부터 loop를 돌며, gamelevel을 찾음
        {
            var gameLevel = rootObject.GetComponent<GameLevel>();
            if (gameLevel != null)
            {
                if (gameLevel.HasLevelObject(levelObject)) //이미 등록 되어 있는 경우
                {
                    Debug.LogWarning(o.name + " is already registered.", o);
                    return;
                }

                Undo.RecordObject(gameLevel, "Register Level Object.");
                gameLevel.RegisterLevelObject(levelObject); //finally, 제대로 등록되어야 할 경우, undo와 log 표시
                Debug.Log(
                    o.name + " registered to game level " +
                    gameLevel.name + " in scene " + o.scene.name + ".", o
                );
                return;
            }
        }
        Debug.LogWarning(o.name + " isn't part of a game level.", o);
    }
}