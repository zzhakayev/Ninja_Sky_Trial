using UnityEngine;

public class HealingPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Health health = other.GetComponent<Health>();
        if (health == null || health.CurrentHP >= health.maxHP) return;
        health.Heal(1);
        Destroy(gameObject);
    }
}
