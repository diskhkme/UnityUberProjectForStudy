using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyFactory originFactory;

    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factory!");
            originFactory = value;
        }
    }

    internal void SpawnOn(GameTile spawnPoint)
    {
        transform.localPosition = spawnPoint.transform.localPosition;
    }
}