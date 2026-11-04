using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public Vector3 direction;
    public float speed = 5f;

    void Start()
    {
        Destroy(gameObject, 7f);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Health health = other.GetComponent<Health>();
        if (health != null) health.TakeDamage(1);
        Destroy(gameObject);
    }
}
