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

    float attackDistanceThreshold = .5f;

    float timeBetweenAttacks = 1f;
    float nextAttackTime;

    float myCollisionRadius;
    float targetCollisionRadius;

    protected override void Start()
    {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color;

        target = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = State.Chasing;

        myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        targetCollisionRadius = GetComponent<CapsuleCollider>().radius;

        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        if(Time.time > nextAttackTime) //time 체크를 거리 체크보다 먼저 하는것이 효율적
        {
            float sqrDistanceToTarget = Vector3.SqrMagnitude(target.position - transform.position);
            if (sqrDistanceToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
            {
                nextAttackTime = Time.time + timeBetweenAttacks;
                StartCoroutine(Attack());
            }
        }

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

        while (percent <= 1)
        {
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

        while(target != null)
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
