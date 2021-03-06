﻿using UnityEngine;

public class CompositeSpawnZone : SpawnZone
{
    
    [SerializeField] SpawnZone[] spawnZones;

    [SerializeField] bool sequencial; //spawnZone을 순차적으로 사용하는 옵션
    int nextSequentialIndex;

    [SerializeField] bool overrideConfig;

    public override Vector3 SpawnPoint
    {
        get
        {
            int index;
            if(sequencial)
            {
                index = nextSequentialIndex++;
                if(nextSequentialIndex >= spawnZones.Length)
                {
                    nextSequentialIndex = 0;
                }
            }
            else
            {
                index = Random.Range(0, spawnZones.Length);
            }
            return spawnZones[index].SpawnPoint;
        }
    }

    //composite spawn zone의 경우 다른 처리가 필요하므로 override
    public override void SpawnShape()
    {
        //하위 zone들에 같은 값을 적용하는 옵션 제공.
        if(overrideConfig)
        {
            base.SpawnShape();
        }
        else
        {
            int index;
            if (sequencial)
            {
                index = nextSequentialIndex++;
                if (nextSequentialIndex >= spawnZones.Length)
                {
                    nextSequentialIndex = 0;
                }
            }
            else
            {
                index = Random.Range(0, spawnZones.Length);
            }
            spawnZones[index].SpawnShape();
        }
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer); //composite도 spawnzone의 progress 저장 해야 함
        writer.Write(nextSequentialIndex);
    }

    public override void Load(GameDataReader reader)
    {
        if(reader.Version >= 7)
        {
            base.Load(reader);
        }
        nextSequentialIndex = reader.ReadInt();
    }

}
