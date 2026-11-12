
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrollerAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private LayerMask wallsMask;

    [Header("Vision")]
    [SerializeField] private float viewRange = 7f;
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private float forgetTime = 2f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float windUp = 0.5f;
    [SerializeField] private float recovery = 0.6f;

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
        if (player == null || patrolPoints.Length < 2)
        {
            Debug.LogError("Patroller: Assign Player and two Patrol Points!");
            enabled = false;
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Patroller: Agent is not on NavMesh!");
            enabled = false;
            return;
        }

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
            agent.stoppingDistance = attackRange * 0.8f;
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

            if (DistanceToPlayer() <= attackRange)
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

        // Warning before the strike
        if (!attacked && stateTimer >= windUp)
        {
            attacked = true;

            if (DistanceToPlayer() <= attackRange &&
                CanSeePlayer())
            {
                Debug.Log("Patroller attack HIT!");
                Health health = player.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(1);}
            }
            else
            {
                Debug.Log("Patroller attack MISSED!");
            }
        }

        // Recovery before chasing again
        if (stateTimer >= windUp + recovery)
            SetState(State.Chase);
    }

    private bool CanSeePlayer()
    {
        Vector3 eye = transform.position + Vector3.up;
        Vector3 target = player.position + Vector3.up;
        Vector3 direction = target - eye;

        float distance = direction.magnitude;

        if (distance > viewRange || distance < 0.01f)
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
