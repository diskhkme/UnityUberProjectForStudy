//아래 코드들은 gamelevel 데이터를 필요로 하긴 하지만, build 후에는 사용하지 않으므로, partial을 사용해서 코드를 분리.

#if UNITY_EDITOR

using UnityEngine;

//partial로 사용할 것은 ClassName.Purpose.cs가 좋음
partial class GameLevel //public, 상속관계 등 다시 적어줄 필요 없음
{
    public bool HasMissingLevelObjects //levelobject list가 에디터에서 빵꾸(?)났을 때 처리하기 위해
    {
        get
        {
            if (levelObjects != null)
            {
                for (int i = 0; i < levelObjects.Length; i++)
                {
                    if (levelObjects[i] == null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public void RemoveMissingLevelObjects()
    {
        if(Application.isPlaying)
        {
            //플레이 도중에는 변경 못하도록 방지.
            Debug.LogError("Do not invoke in play mode!");
            return;
        }
        int holes = 0;
        for (int i = 0; i < levelObjects.Length - holes; i++)
        {
            if (levelObjects[i] == null)
            {
                holes += 1;
                System.Array.Copy(levelObjects, i + 1, levelObjects, i, levelObjects.Length - i - holes);
                i -= 1;
            }
        }
        System.Array.Resize(ref levelObjects, levelObjects.Length - holes);
    }

    public void RegisterLevelObject(GameLevelObject o)
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Do not invoke in play mode!");
            return;
        }
        if(HasLevelObject(o))
        {
            return;
        }

        if (levelObjects == null)
        {
            levelObjects = new GameLevelObject[] { o };
        }
        else
        {
            System.Array.Resize(ref levelObjects, levelObjects.Length + 1);
            levelObjects[levelObjects.Length - 1] = o;
        }
    }

    public bool HasLevelObject(GameLevelObject o)
    {
        if (levelObjects != null)
        {
            for (int i = 0; i < levelObjects.Length; i++)
            {
                if (levelObjects[i] == o)
                {
                    return true;
                }
            }
        }
        return false;
    }


    
}

#endif