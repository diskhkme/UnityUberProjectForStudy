using UnityEngine;
using UnityEngine.SceneManagement;

//이전과 마찬가지로 factory를 scriptable object로 정의
[CreateAssetMenu]
public class GameTileContentFactory : ScriptableObject
{
    Scene contentScene;
    [SerializeField] GameTileContent destinationPrefab = default;
    [SerializeField] GameTileContent emptyPrefab = default;
    [SerializeField] GameTileContent wallPrefab = default;

    public void Reclaim(GameTileContent content)
    {
        Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed!");
        Destroy(content.gameObject);
    }

    GameTileContent Get(GameTileContent prefab)
    {
        GameTileContent instance = Instantiate(prefab); //객체의 생성을 담당함
        instance.OriginFactory = this; //객체에는, 어느 factory에서 생긴 것인지 등록해 놓음
        MoveToFactoryScene(instance.gameObject);
        return instance;
    }

    //public Get(). 요청하는 타입의 컨텐츠를 생성하고 생성된 것을 반환
    public GameTileContent Get(GameTileContentType type)
    {
        switch(type)
        {
            case GameTileContentType.Destination: return Get(destinationPrefab);
            case GameTileContentType.Empty: return Get(emptyPrefab);
            case GameTileContentType.Wall: return Get(wallPrefab);
        }
        Debug.Assert(false, "Unsupported type: " + type);
        return null;
    }

    void MoveToFactoryScene(GameObject o) //특정 게임오브젝트를 특정 scene으로 옮김
    {
        if(!contentScene.isLoaded)
        {
            if(Application.isEditor)
            {
                contentScene = SceneManager.GetSceneByName(name);
                if(!contentScene.isLoaded)
                {
                    contentScene = SceneManager.CreateScene(name);
                }
            }
            else
            {
                contentScene = SceneManager.CreateScene(name);
            }
        }
        SceneManager.MoveGameObjectToScene(o, contentScene);
    }
}
