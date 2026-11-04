using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 12f;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = transform.forward * speed;
        }
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter(Collider other)
    {
        BossEnemy boss = other.GetComponentInParent<BossEnemy>();
        if (boss != null)
        {
            boss.TakeHit();
            Destroy(gameObject);
        }
    }
}
