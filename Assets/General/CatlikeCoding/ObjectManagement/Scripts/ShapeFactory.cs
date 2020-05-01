using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Scriptable Object 사용!
[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{
    [SerializeField] Shape[] prefabs;
    [SerializeField] Material[] materials;
    [SerializeField] bool recycle;

    List<Shape>[] pools;
    //pool 객체를 정리하기 위해서는 1) root object를 만들거나, 2) object 정보를 담은 scene을 만들면 됨
    //1의 경우, 객체가 activate/deactive 될때 hierarchy의 다른 오브젝트들에게 신호를 보내기 때문에 performance에 부정적 영향이 있음.
    //따라서 2의 방법으로 구현
    Scene poolScene;

    //shape id처럼 factoryid도 자동 생성. scriptable object는 editor가 살아있는동안 살아있음. 
    [System.NonSerialized]
    int factoryId = int.MinValue;
    public int FactoryId
    {
        get
        {
            return factoryId;
        }
        set
        {
            if(factoryId == int.MinValue && value != int.MinValue)
            {
                factoryId = value;
            }
            else
            {
                Debug.Log("Not allwed to change factoryId.");
            }
        }

    }

    public Shape Get(int shapeId = 0, int materialId = 0)
    {
        Shape instance;
        if(recycle)
        {
            if(pools == null)
            {
                CreatePools();
            }
            List<Shape> pool = pools[shapeId]; //해당 shapeId pool ref
            int lastIndex = pool.Count - 1; //pool에 저장되어있는 마지막 객체 인덱스
            if(lastIndex >= 0) //pool에 무언가 있을 떄
            {
                instance = pool[lastIndex]; //마지막 객체
                pool.RemoveAt(lastIndex); //activa한 마지막 객체는 pool에서 제거
            }
            else //pool에 아무것도 없을 때
            {
                instance = Instantiate(prefabs[shapeId]);
                instance.OriginFactory = this;
                instance.ShapeId = shapeId;
                SceneManager.MoveGameObjectToScene(instance.gameObject, poolScene); //특정 scene으로 gameobject를 옮기는 방법
            }
            instance.gameObject.SetActive(true); //마지막 객체를 active로
        }
        else
        {
            instance = Instantiate(prefabs[shapeId]);
            instance.ShapeId = shapeId;
        }
        
        instance.SetMaterial(materials[materialId], materialId);
        return instance;
    }

    public Shape GetRandom()
    {
        return Get(Random.Range(0, prefabs.Length), Random.Range(0,materials.Length));
    }

    void CreatePools()
    {
        pools = new List<Shape>[prefabs.Length]; //Shape 종류만큼 pool list 배열 만듬
        for(int i=0;i<pools.Length;i++)
        {
            pools[i] = new List<Shape>();
        }

        //recompilation 과정에서 생기는 문제 해결을 위함(100% 이해 못한 듯...). 빌드에서는 문제 없다 함.
        if(Application.isEditor)
        {
            poolScene = SceneManager.GetSceneByName(name);
            if (poolScene.isLoaded)
            {
                GameObject[] rootObjects = poolScene.GetRootGameObjects();
                for(int i=0;i<rootObjects.Length;i++)
                {
                    Shape pooledShape = rootObjects[i].GetComponent<Shape>();
                    if(!pooledShape.gameObject.activeSelf)
                    {
                        pools[pooledShape.ShapeId].Add(pooledShape);
                    }
                }
                return;
            }
        }
        
        poolScene = SceneManager.CreateScene(this.name);
    }

    public void Reclaim(Shape shapeToRecycle)
    {
        if(shapeToRecycle.OriginFactory != this)
        {
            Debug.LogError("Tried to reclaim shape with wrong factory.");
            return;
        }

        if(recycle) //플레이 도중에 recycle을 바꾸면, pool에 아무것도 없을 떄 reclaim되는 오류가 생길 수 있으므로 확인차 작성
        {
            if(pools == null)
            {
                CreatePools();
            }
            pools[shapeToRecycle.ShapeId].Add(shapeToRecycle); //받은 shape을 pool에 추가(당연히 마지막 index에 추가됨)
            shapeToRecycle.gameObject.SetActive(false);
        }
        else
        {
            Destroy(shapeToRecycle.gameObject);
        }
    }
}
