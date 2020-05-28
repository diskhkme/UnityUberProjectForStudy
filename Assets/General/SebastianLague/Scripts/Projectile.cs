using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    float speed = 10f;
    float damage = 1f;

    float lifetime = 3f;
    float skinWidth = 0.1f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        //적이 캐릭터를 파고들어서 생기는 문제 해결. 총알이 생성되면 그 총알이 겹치는 collider 체크
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);

        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0]);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    private void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //skinWidth로 적과 총알이 동시에 움직이면서 생기는 intersection 문제를 해결
        if(Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if(damageableObject != null)
        {
            damageableObject.TakeHit(damage, hit);
        }
        GameObject.Destroy(gameObject);
    }

    void OnHitObject(Collider c)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }
        GameObject.Destroy(gameObject);
    }
}
