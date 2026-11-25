using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public Vector3 offset = new Vector3(2.5f, 0f, 0f);
    public float speed = 1.8f;
    private Vector3 origin;
    private Transform player;
    private bool warned;
    [SerializeField] private float warningDistance = 3f;

    void Start()
    {
        origin = transform.position;
        var target = GameObject.FindGameObjectWithTag("Player");
        if (target != null) player = target.transform;
    }

    void Update()
    {
        transform.position = origin + offset * Mathf.Sin(Time.time * speed);
        transform.Rotate(Vector3.up, 80f * Time.deltaTime);
        if (player == null || Time.timeScale == 0f) return;
        float distance = Vector3.Distance(player.position, transform.position);
        if (!warned && distance <= warningDistance)
        {
            warned = true;
            GameAudio.EnemySound("PatrolWarn", transform.position);
        }
        else if (distance > warningDistance + 0.5f) warned = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            GameAudio.EnemySound("EnemyAttack", transform.position, 0.75f);
            health.TakeDamage(1);
        }
    }
}
