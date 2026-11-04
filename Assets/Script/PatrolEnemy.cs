using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public Vector3 offset = new Vector3(2.5f, 0f, 0f);
    public float speed = 1.8f;
    private Vector3 origin;

    void Start()
    {
        origin = transform.position;
    }

    void Update()
    {
        transform.position = origin + offset * Mathf.Sin(Time.time * speed);
        transform.Rotate(Vector3.up, 80f * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Health health = other.GetComponent<Health>();
        if (health != null) health.TakeDamage(1);
    }
}
