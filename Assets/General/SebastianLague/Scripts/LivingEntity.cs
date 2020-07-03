using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    protected float health;
    protected bool dead;

    //적이 죽었을 경우, SpawnSystem이 알아야 하는데, 서로 의존성을 주는 것이 좋지 않으므로 event 사용
    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public virtual void TakeHit(float damage, Vector3 hitpoint, Vector3 hitDirection)
    {
        //hit 관련해서는 나중에 추가
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    [ContextMenu("Self Destruct")] //스크립트를 우클릭하면 메뉴가 생겨서 바로 실행 가능!
    public virtual void Die() //플레이어 죽음 시에만 이펙트를 재생하도록 override
    {
        dead = true;
        if(OnDeath != null)
        {
            OnDeath();
        }
        Destroy(gameObject);
    }
}
