
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrollerAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack }

    [Header("Enemy Data")]
    [SerializeField] private EnemyStats stats;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private LayerMask wallsMask;

    [Header("Vision")]
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private float forgetTime = 2f;

    [Header("Debug")]
    [SerializeField] private State currentState;

    private NavMeshAgent agent;
    private Renderer body;
    private MaterialPropertyBlock colorBlock;

    private int patrolIndex;
    private float stateTimer;
    private float lostTimer;
    private float scanTimer;
    private float repathTimer;
    private bool seesPlayer;
    private bool attacked;
    private Vector3 lastSeenPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        body = GetComponentInChildren<Renderer>();
        colorBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        if (stats == null)
        {
            Debug.LogError("Patroller: EnemyStats not assigned!");
            enabled = false;
            return;
        }

        if (player == null || patrolPoints == null ||
            patrolPoints.Length < 2)
        {
            Debug.LogError("Patroller: Assign Player and Patrol Points!");
            enabled = false;
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Patroller: Agent is not on NavMesh!");
            enabled = false;
            return;
        }

        // Movement speed comes from ScriptableObject.
        agent.speed = stats.moveSpeed;

        SetState(State.Patrol);
    }

    private void Update()
    {
        scanTimer -= Time.deltaTime;
        repathTimer -= Time.deltaTime;
        stateTimer += Time.deltaTime;

        if (scanTimer <= 0f)
        {
            seesPlayer = CanSeePlayer();
            scanTimer = 0.15f;
        }

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                if (seesPlayer)
                    SetState(State.Chase);
                break;

            case State.Chase:
                Chase();
                break;

            case State.Attack:
                Attack();
                break;
        }
    }

    private void SetState(State next)
    {
        currentState = next;
        stateTimer = 0f;
        if (next == State.Attack) GameAudio.EnemySound("PatrolWarn", transform.position);
        attacked = false;

        if (next == State.Patrol)
        {
            agent.isStopped = false;
            agent.stoppingDistance = 0.15f;
            agent.SetDestination(patrolPoints[patrolIndex].position);
            SetColor(Color.yellow);
        }
        else if (next == State.Chase)
        {
            lostTimer = 0f;
            repathTimer = 0f;
            agent.isStopped = false;
            agent.stoppingDistance = stats.attackRange * 0.8f;
            SetColor(Color.cyan);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
            SetColor(Color.red);
        }

        Debug.Log("Patroller state: " + next);
    }

    private void Patrol()
    {
        if (agent.pathPending)
            return;

        if (agent.remainingDistance <= 0.3f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    private void Chase()
    {
        if (seesPlayer)
        {
            lostTimer = 0f;
            lastSeenPosition = player.position;

            if (DistanceToPlayer() <= stats.attackRange)
            {
                SetState(State.Attack);
                return;
            }
        }
        else
        {
            lostTimer += Time.deltaTime;

            if (lostTimer >= forgetTime)
            {
                SetState(State.Patrol);
                return;
            }
        }

        if (repathTimer <= 0f)
        {
            agent.SetDestination(lastSeenPosition);
            repathTimer = 0.2f;
        }
    }

    private void Attack()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                360f * Time.deltaTime
            );
        }

        // Warning time comes from EnemyStats.
        if (!attacked && stateTimer >= stats.windUpTime)
        {
            attacked = true;
            GameAudio.EnemySound("EnemyAttack", transform.position, 0.75f);

            if (DistanceToPlayer() <= stats.attackRange &&
                CanSeePlayer())
            {
                Debug.Log("Patroller attack HIT!");

                Health health = player.GetComponent<Health>();

                if (health != null)
                    health.TakeDamage(stats.damage);
            }
            else
            {
                Debug.Log("Patroller attack MISSED!");
            }
        }

        // Cooldown comes from EnemyStats.
        if (stateTimer >= stats.windUpTime + stats.attackCooldown)
            SetState(State.Chase);
    }

    private bool CanSeePlayer()
    {
        Vector3 eye = transform.position + Vector3.up;
        Vector3 target = player.position + Vector3.up;
        Vector3 direction = target - eye;

        float distance = direction.magnitude;

        if (distance > stats.viewRange || distance < 0.01f)
            return false;

        Vector3 flatDirection = direction;
        flatDirection.y = 0f;

        if (Vector3.Angle(transform.forward, flatDirection)
            > viewAngle / 2f)
            return false;

        if (Physics.Raycast(
            eye,
            direction.normalized,
            distance,
            wallsMask,
            QueryTriggerInteraction.Ignore))
            return false;

        return true;
    }

    private float DistanceToPlayer()
    {
        Vector3 difference = player.position - transform.position;
        difference.y = 0f;
        return difference.magnitude;
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
        if (stats == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.viewRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.attackRange);
    }
}
