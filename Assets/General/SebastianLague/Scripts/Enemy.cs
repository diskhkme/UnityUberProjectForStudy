using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    NavMeshAgent pathfinder;
    Transform target; //player

    private void Start()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        //pathfinder.SetDestination(target.position); //성능 문제 발생 가능
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = .25f;

        while(target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, 0f, target.position.z);
            pathfinder.SetDestination(targetPosition);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
