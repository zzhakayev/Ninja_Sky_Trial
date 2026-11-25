
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ShooterAI : MonoBehaviour
{
    public enum State { Idle, KeepDistance, Aim, Cooldown }

    [Header("Enemy Data")]
    [SerializeField] private EnemyStats stats;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask wallsMask;

    [Header("Detection")]
    [SerializeField] private float forgetTime = 2f;

    [Header("Movement")]
    [SerializeField] private float preferredDistance = 5f;
    [SerializeField] private float distanceTolerance = 0.8f;

    [Header("Shooting")]
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float projectileLifetime = 4f;
    [SerializeField] private float hitRadius = 0.65f;

    [Header("Debug")]
    [SerializeField] private State currentState;

    private NavMeshAgent agent;
    private Renderer body;
    private MaterialPropertyBlock colorBlock;
    private Camera gameCamera;

    private float stateTimer;
    private float scanTimer;
    private float repathTimer;
    private float lostTimer;
    private bool seesPlayer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        body = GetComponentInChildren<Renderer>();
        colorBlock = new MaterialPropertyBlock();
        gameCamera = Camera.main;
    }

    private void Start()
    {
        if (stats == null)
        {
            Debug.LogError("Shooter: Assign Shooter_Stats!");
            enabled = false;
            return;
        }

        if (player == null || !agent.isOnNavMesh)
        {
            Debug.LogError(
                "Shooter: assign Player and check NavMesh!"
            );
            enabled = false;
            return;
        }

        agent.speed = stats.moveSpeed;
        agent.stoppingDistance = 0.15f;

        SetState(State.Idle);
    }

    private void Update()
    {
        stateTimer += Time.deltaTime;
        scanTimer -= Time.deltaTime;
        repathTimer -= Time.deltaTime;

        if (scanTimer <= 0f)
        {
            seesPlayer = CanSeePlayer();
            scanTimer = 0.15f;
        }

        switch (currentState)
        {
            case State.Idle:
                if (seesPlayer)
                    SetState(State.KeepDistance);
                break;

            case State.KeepDistance:
                KeepDistance();
                break;

            case State.Aim:
                Aim();
                break;

            case State.Cooldown:
                if (stateTimer >= stats.attackCooldown)
                {
                    SetState(seesPlayer
                        ? State.KeepDistance
                        : State.Idle);
                }
                break;
        }
    }

    private void SetState(State next)
    {
        currentState = next;
        stateTimer = 0f;
        if (next == State.Aim) GameAudio.EnemySound("ShooterWarn", transform.position);

        if (next == State.Idle)
        {
            agent.isStopped = true;
            agent.ResetPath();
            SetColor(Color.blue);
        }
        else if (next == State.KeepDistance)
        {
            agent.isStopped = false;
            repathTimer = 0f;
            SetColor(Color.cyan);
        }
        else if (next == State.Aim)
        {
            agent.isStopped = true;
            agent.ResetPath();
            SetColor(Color.red);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
            SetColor(Color.magenta);
        }

        Debug.Log("Shooter state: " + next);
    }

    private void KeepDistance()
    {
        if (!seesPlayer)
        {
            lostTimer += Time.deltaTime;

            if (agent.hasPath)
                agent.ResetPath();

            if (lostTimer >= forgetTime)
                SetState(State.Idle);

            return;
        }

        lostTimer = 0f;

        Vector3 away = transform.position - player.position;
        away.y = 0f;

        float distance = away.magnitude;

        if (distance > 0.01f)
        {
            LookAtPlayer();

            if (repathTimer <= 0f)
            {
                if (Mathf.Abs(distance - preferredDistance)
                    > distanceTolerance)
                {
                    Vector3 desiredPosition =
                        player.position +
                        away.normalized * preferredDistance;

                    if (NavMesh.SamplePosition(
                        desiredPosition,
                        out NavMeshHit hit,
                        2f,
                        agent.areaMask))
                    {
                        agent.SetDestination(hit.position);
                    }
                }
                else if (agent.hasPath)
                {
                    agent.ResetPath();
                }

                repathTimer = 0.2f;
            }
        }

        // Attack only while visible on the screen.
        if (stateTimer >= 0.35f &&
            distance <= stats.viewRange &&
            IsVisibleOnScreen())
        {
            SetState(State.Aim);
        }
    }

    private void Aim()
    {
        LookAtPlayer();

        if (stateTimer >= stats.windUpTime)
        {
            FireProjectile();
            SetState(State.Cooldown);
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 eye = transform.position + Vector3.up;
        Vector3 target = player.position + Vector3.up;
        Vector3 direction = target - eye;

        float distance = direction.magnitude;

        if (distance > stats.viewRange || distance < 0.01f)
            return false;

        return !Physics.Raycast(
            eye,
            direction.normalized,
            distance,
            wallsMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private bool IsVisibleOnScreen()
    {
        if (gameCamera == null)
            return false;

        Vector3 point = gameCamera.WorldToViewportPoint(
            transform.position + Vector3.up
        );

        return point.z > 0f &&
               point.x >= 0f && point.x <= 1f &&
               point.y >= 0f && point.y <= 1f;
    }

    private void LookAtPlayer()
    {
        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion rotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            rotation,
            360f * Time.deltaTime
        );
    }

    private void FireProjectile()
    {
        GameAudio.EnemySound("EnemyAttack", transform.position, 0.85f);
        Vector3 origin =
            transform.position + Vector3.up +
            transform.forward * 0.8f;

        Vector3 target =
            player.position + Vector3.up;

        Vector3 direction = (target - origin).normalized;

        GameObject bullet =
            GameObject.CreatePrimitive(PrimitiveType.Sphere);

        bullet.name = "ShooterProjectile";
        bullet.transform.position = origin;
        bullet.transform.localScale = Vector3.one * 0.4f;

        Collider bulletCollider =
            bullet.GetComponent<Collider>();

        bulletCollider.enabled = false;

        Renderer bulletRenderer =
            bullet.GetComponent<Renderer>();

        MaterialPropertyBlock block =
            new MaterialPropertyBlock();

        block.SetColor("_BaseColor", Color.red);
        block.SetColor("_Color", Color.red);
        bulletRenderer.SetPropertyBlock(block);

        StartCoroutine(MoveProjectile(bullet, direction));

        Debug.Log("Shooter fired a projectile!");
    }

    private IEnumerator MoveProjectile(
        GameObject bullet,
        Vector3 direction)
    {
        float lifetime = 0f;

        while (lifetime < projectileLifetime &&
               bullet != null)
        {
            Vector3 start = bullet.transform.position;

            float step = projectileSpeed * Time.deltaTime;
            Vector3 movement = direction * step;

            // Stop the projectile at walls.
            if (Physics.Raycast(
                start,
                direction,
                step,
                wallsMask,
                QueryTriggerInteraction.Ignore))
            {
                break;
            }

            Vector3 end = start + movement;

            // Check whether the projectile crosses the player.
            Vector3 playerCenter =
                player.position + Vector3.up;

            float t = Mathf.Clamp01(
                Vector3.Dot(playerCenter - start, movement) /
                Mathf.Max(movement.sqrMagnitude, 0.0001f)
            );

            Vector3 nearest = start + movement * t;

            if (Vector3.Distance(nearest, playerCenter)
                <= hitRadius)
            {
                Debug.Log("Shooter projectile HIT player!");
                Health health = player.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(stats.damage);
                    }
                    break;
            }

            bullet.transform.position = end;

            lifetime += Time.deltaTime;
            yield return null;
        }

        Destroy(bullet);
    }

    private void SetColor(Color color)
    {
        if (body == null)
            return;

        body.GetPropertyBlock(colorBlock);
        colorBlock.SetColor("_BaseColor", color);
        colorBlock.SetColor("_Color", color);
        body.SetPropertyBlock(colorBlock);
    }

    private void OnDrawGizmosSelected()
    {
        if (stats == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stats.viewRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            preferredDistance
        );
    }
}
