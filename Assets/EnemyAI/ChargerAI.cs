
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ChargerAI : MonoBehaviour
{
    public enum State { Idle, WindUp, Dash, Stunned }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask wallsMask;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 6f;

    [Header("Attack")]
    [SerializeField] private float windUpTime = 0.7f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float maxDashDistance = 5f;
    [SerializeField] private float hitRange = 1.1f;
    [SerializeField] private float stunTime = 1f;
    [SerializeField] private float cooldown = 1.5f;

    [Header("Debug")]
    [SerializeField] private State currentState;

    private NavMeshAgent agent;
    private Renderer body;
    private MaterialPropertyBlock colorBlock;
    private Camera gameCamera;

    private float stateTimer;
    private float dashTravelled;
    private Vector3 dashDirection;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        body = GetComponentInChildren<Renderer>();
        colorBlock = new MaterialPropertyBlock();
        gameCamera = Camera.main;

        agent.updateRotation = false;
    }

    private void Start()
    {
        if (player == null || !agent.isOnNavMesh)
        {
            Debug.LogError(
                "Charger: assign Player and check NavMesh!"
            );
            enabled = false;
            return;
        }

        SetState(State.Idle);
    }

    private void Update()
    {
        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
                break;

            case State.WindUp:
                UpdateWindUp();
                break;

            case State.Dash:
                UpdateDash();
                break;

            case State.Stunned:
                if (stateTimer >= stunTime)
                    SetState(State.Idle);
                break;
        }
    }

    private void UpdateIdle()
    {
        SetColor(Color.green);

        if (stateTimer >= cooldown &&
            CanSeePlayer() &&
            IsVisibleOnScreen())
        {
            SetState(State.WindUp);
        }
    }

    private void UpdateWindUp()
    {
        LookAtPlayer();

        // Flash red to warn the player.
        bool flash = Mathf.FloorToInt(
            stateTimer * 10f
        ) % 2 == 0;

        SetColor(flash ? Color.red : Color.yellow);

        if (stateTimer >= windUpTime)
        {
            if (!IsVisibleOnScreen())
            {
                SetState(State.Idle);
                return;
            }

            dashDirection =
                player.position - transform.position;
            dashDirection.y = 0f;

            if (dashDirection.sqrMagnitude < 0.01f)
            {
                SetState(State.Stunned);
                return;
            }

            dashDirection.Normalize();
            dashTravelled = 0f;

            SetState(State.Dash);
        }
    }

    private void UpdateDash()
    {
        SetColor(Color.red);

        float step = Mathf.Min(
            dashSpeed * Time.deltaTime,
            maxDashDistance - dashTravelled
        );

        if (step <= 0f)
        {
            SetState(State.Stunned);
            return;
        }

        Vector3 nextPosition =
            transform.position + dashDirection * step;

        // Stop at a NavMesh boundary.
        if (agent.Raycast(
            nextPosition, out NavMeshHit navHit))
        {
            SetState(State.Stunned);
            return;
        }

        // Stop at physical walls.
        Vector3 eye = transform.position + Vector3.up;

        if (Physics.SphereCast(
            eye,
            0.25f,
            dashDirection,
            out RaycastHit wallHit,
            step + 0.1f,
            wallsMask,
            QueryTriggerInteraction.Ignore))
        {
            SetState(State.Stunned);
            return;
        }

        // Move directly, rather than pathfinding around walls.
        agent.Move(dashDirection * step);
        dashTravelled += step;

        Vector3 difference =
            player.position - transform.position;
        difference.y = 0f;

        if (difference.magnitude <= hitRange)
        {
            Debug.Log("Charger attack HIT player!");
           Health health = player.GetComponent<Health>();
           if (health != null)
           {
            health.TakeDamage(1);
            }
            
            SetState(State.Stunned);
            return;
        }

        if (dashTravelled >= maxDashDistance)
        {
            Debug.Log("Charger finished dash!");
            SetState(State.Stunned);
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 difference =
            player.position - transform.position;
        difference.y = 0f;

        if (difference.magnitude > detectionRange)
            return false;

        Vector3 eye = transform.position + Vector3.up;
        Vector3 target = player.position + Vector3.up;
        Vector3 direction = target - eye;

        return !Physics.Raycast(
            eye,
            direction.normalized,
            direction.magnitude,
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

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            720f * Time.deltaTime
        );
    }

    private void SetState(State next)
    {
        currentState = next;
        stateTimer = 0f;

        agent.ResetPath();

        if (next == State.Idle)
            SetColor(Color.green);
        else if (next == State.WindUp)
            SetColor(Color.yellow);
        else if (next == State.Dash)
            SetColor(Color.red);
        else
            SetColor(Color.gray);

        Debug.Log("Charger state: " + next);
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
        Gizmos.DrawWireSphere(
            transform.position, detectionRange
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position, hitRange
        );
    }
}
