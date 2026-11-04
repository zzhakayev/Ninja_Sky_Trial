using System.Collections;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    public int maxHP = 8;
    public int CurrentHP { get; private set; }
    public Transform target;
    public MidtermLevel level;
    private Renderer bossRenderer;
    private AudioSource audioSource;
    private AudioClip hitClip;
    private Color originalColor;
    private bool defeated;

    void Start()
    {
        CurrentHP = maxHP;
        bossRenderer = GetComponent<Renderer>();
        if (bossRenderer != null) originalColor = bossRenderer.material.color;
        audioSource = GetComponent<AudioSource>();
        hitClip = Resources.Load<AudioClip>("Audio/BossHit");
        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        if (target == null || defeated) return;
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion look = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 2f);
        }
        transform.localScale = Vector3.one * (1.55f + Mathf.Sin(Time.time * 3f) * 0.04f);
    }

    public void TakeHit()
    {
        if (defeated || CurrentHP <= 0) return;
        CurrentHP--;
        if (audioSource != null && hitClip != null) audioSource.PlayOneShot(hitClip, 0.8f);
        if (CurrentHP <= 0)
        {
            defeated = true;
            if (level != null) level.OnBossDefeated();
            Destroy(gameObject);
            return;
        }
        if (bossRenderer != null) StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        bossRenderer.material.color = Color.white;
        yield return new WaitForSeconds(0.12f);
        if (bossRenderer != null) bossRenderer.material.color = originalColor;
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(2f);
        while (!defeated && target != null)
        {
            bool secondPhase = CurrentHP <= maxHP / 2;
            Vector3 mark = target.position;
            GameObject warning = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            warning.name = "Guardian Warning Marker";
            warning.transform.position = new Vector3(mark.x, Mathf.Max(0.08f, mark.y - 0.53f), mark.z);
            warning.transform.localScale = new Vector3(1.4f, 0.035f, 1.4f);
            Renderer markRenderer = warning.GetComponent<Renderer>();
            if (markRenderer != null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                if (shader != null)
                {
                    markRenderer.material = new Material(shader);
                    markRenderer.material.color = new Color(1f, 0.18f, 0.23f);
                }
            }
            Collider markerCollider = warning.GetComponent<Collider>();
            if (markerCollider != null) Destroy(markerCollider);
            yield return new WaitForSeconds(secondPhase ? 0.45f : 0.72f);
            if (warning != null) Destroy(warning);
            if (target == null || defeated) yield break;
            Vector3 direction = (target.position + Vector3.up * 0.2f - transform.position).normalized;
            int count = secondPhase ? 3 : 1;
            for (int i = 0; i < count; i++)
            {
                float angle = secondPhase ? (i - 1) * 13f : 0f;
                SpawnShot(Quaternion.Euler(0f, angle, 0f) * direction, secondPhase ? 6f : 5.2f);
            }
            yield return new WaitForSeconds(secondPhase ? 1.15f : 1.75f);
        }
    }

    void SpawnShot(Vector3 direction, float speed)
    {
        GameObject shot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shot.name = "Boss Projectile";
        shot.transform.position = transform.position + direction * 1.25f;
        shot.transform.localScale = Vector3.one * 0.48f;
        Collider col = shot.GetComponent<Collider>();
        col.isTrigger = true;
        Rigidbody rb = shot.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        Renderer renderer = shot.GetComponent<Renderer>();
        if (renderer != null) renderer.material.color = new Color(1f, 0.22f, 0.25f);
        BossProjectile projectile = shot.AddComponent<BossProjectile>();
        projectile.direction = direction;
        projectile.speed = speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null) health.TakeDamage(1);
        }
    }
}
