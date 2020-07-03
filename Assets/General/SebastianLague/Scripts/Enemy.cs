using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State
    {
        Idle, Chasing, Attacking
    };

    public ParticleSystem deathEffect;

    NavMeshAgent pathfinder;
    Transform target; //player
    State currentState;
    Material skinMaterial;
    Color originalColor;
    LivingEntity targetEntity;

    float attackDistanceThreshold = .5f;

    float timeBetweenAttacks = 1f;
    float nextAttackTime;
    float damage = 1f;

    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    private void Awake()
    {
        //SetCharacteristic의 missing reference 방지를 위해 참조 생성을 Awake로 옮겨옴
        pathfinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = GetComponent<CapsuleCollider>().radius;
        }
    }

    protected override void Start()
    {
        base.Start();

        if(hasTarget)
        {
            targetEntity.OnDeath += OnTargetDeath;
            currentState = State.Chasing;

            StartCoroutine(UpdatePath());
        }
        
    }

    private void Update()
    {
        if(hasTarget)
        {
            if (Time.time > nextAttackTime) //time 체크를 거리 체크보다 먼저 하는것이 효율적
            {
                float sqrDistanceToTarget = Vector3.SqrMagnitude(target.position - transform.position);
                if (sqrDistanceToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    AudioManager.instance.PlaySound("EnemyAttack", transform.position);
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
        
    }

    public override void TakeHit(float damage, Vector3 hitpoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        //death effect 실행
        if(damage >= health)
        {
            AudioManager.instance.PlaySound("EnemyDeath", transform.position);
            Destroy(Instantiate(deathEffect.gameObject, hitpoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject,
                    deathEffect.main.startLifetime.constant);
        }
        base.TakeHit(damage, hitpoint, hitDirection);
    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
    {
        pathfinder.speed = moveSpeed;
        if(hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }

        startingHealth = enemyHealth;

        //particle의 색도 같이 바꾸기 위해, sharedMaterial로 바꿈
        skinMaterial = GetComponent<Renderer>().sharedMaterial;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false; //공격 중에는 pathfinding안함

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float percent = 0;
        float attackSpeed = 3;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;
        
        while (percent <= 1)
        {
            if(percent >= 0.5f && !hasAppliedDamage) //갔다 돌아오는 것 까지가 한 루프이므로 0.5가 플레이어에게 최대한 다가갔을 시점
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = 4f * (-percent * percent + percent); //4(-x^2 + x)
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;

        
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = .25f;

        while(hasTarget)
        {
            if(currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2f);

                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
