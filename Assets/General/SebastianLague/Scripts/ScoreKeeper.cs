using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static int score { get; private set; }
    float lastEnemyKillTime;
    int streakcount;
    float streakExpireTime = 1f;
    
    
    private void Start()
    {
        Enemy.OnDeathStatic += OnEnemyKilled;
        //두번 등록되지 않도록 조심해야 함. 플레어가 죽을때 다시 unsubscribe하도록..
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
    }

    void OnEnemyKilled()
    {
        if(Time.time <lastEnemyKillTime + streakExpireTime)
        {
            streakcount++;
        }
        else
        {
            streakcount = 0;
        }

        lastEnemyKillTime = Time.time;

        score += 5 + (int)Mathf.Pow(2, streakcount);
    }

    void OnPlayerDeath()
    {
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }
}
