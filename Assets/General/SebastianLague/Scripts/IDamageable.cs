using UnityEngine;

public interface IDamageable
{
    void TakeHit(float damage, RaycastHit hit); //by bullet
    void TakeDamage(float damage); //by hitting
}
