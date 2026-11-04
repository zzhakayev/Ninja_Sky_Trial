
using UnityEngine;

public class Hazard : MonoBehaviour
{
    protected virtual void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        Health health = other.gameObject.GetComponent<Health>();

        if (health != null)
        {
            OnHitPlayer(health);
        }

        Destroy(gameObject);
    }

    protected virtual void OnHitPlayer(Health player)
    {
        player.TakeDamage(1);
    }
}
