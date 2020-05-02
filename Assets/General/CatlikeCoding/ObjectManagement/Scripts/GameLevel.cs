﻿using UnityEngine;

//상태를 저장해야하므로 PersistableObject로
public class GameLevel : PersistableObject
{
    public static GameLevel Current { get; private set; }

    [SerializeField] SpawnZone spawnZone;

    [SerializeField] PersistableObject[] persistentObjects;

    [SerializeField] int populationLimit;
    public int PopulationLimit
    {
        get
        {
            return populationLimit;
        }
    }

    private void OnEnable()
    {
        Current = this;
        if(persistentObjects == null)
        {
            persistentObjects = new PersistableObject[0];
        }
    }

    public void SpawnShape()
    {
        spawnZone.SpawnShape();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(persistentObjects.Length);
        for(int i=0;i<persistentObjects.Length;i++)
        {
            persistentObjects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int savedCount = reader.ReadInt();
        for(int i=0;i<savedCount;i++)
        {
            persistentObjects[i].Load(reader);
        }
    }
}
