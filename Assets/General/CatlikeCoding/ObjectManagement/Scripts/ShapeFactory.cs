using System.Collections.Generic;
using UnityEngine;


//Scriptable Object 사용!
[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{
    [SerializeField] Shape[] prefabs;
    [SerializeField] Material[] materials;
    [SerializeField] bool recycle;

    List<Shape>[] pools;

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
                instance.ShapeId = shapeId;
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
    }

    public void Reclaim(Shape shapeToRecycle)
    {
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
