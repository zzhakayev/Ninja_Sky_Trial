using UnityEngine;

public class DamageZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Health health = other.GetComponent<Health>();
        if (health != null) health.TakeDamage(1);
    }
}
