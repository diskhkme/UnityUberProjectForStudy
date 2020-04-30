﻿using UnityEngine;

public class CompositeSpawnZone : SpawnZone
{
    
    [SerializeField] SpawnZone[] spawnZones;

    [SerializeField] bool sequencial; //spawnZone을 순차적으로 사용하는 옵션
    int nextSequentialIndex;

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

    public override void Save(GameDataWriter writer)
    {
        //추가적으로 저장할 game state data
        writer.Write(nextSequentialIndex);
    }

    public override void Load(GameDataReader reader)
    {
        nextSequentialIndex = reader.ReadInt();
    }

}
