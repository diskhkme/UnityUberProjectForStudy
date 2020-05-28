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
    

    protected override void Start()
    {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color;

        if(GameObject.FindGameObjectWithTag("Player").transform != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;
            currentState = State.Chasing;
            hasTarget = true;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = GetComponent<CapsuleCollider>().radius;

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
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
        
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
