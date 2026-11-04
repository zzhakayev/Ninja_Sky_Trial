using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    public float force = 14f;
    public float jumpForce = 6f;
    public Animator exitAnimator;

    private Rigidbody rb;
    private Collider playerCollider;
    private Vector2 moveInput;
    private Animator ninjaAnimator;
    private Transform ninjaVisual;
    private MidtermLevel level;
    private AudioSource audioSource;
    private AudioClip jumpClip;
    private int coins;
    private int coinsTotal;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        jumpClip = Resources.Load<AudioClip>("Audio/Jump");
        level = GetComponent<MidtermLevel>();
    }

    void Start()
    {
        coinsTotal = GameObject.FindGameObjectsWithTag("Coin").Length;
    }

    public void ConnectAnimator(Animator animator, Transform visual)
    {
        ninjaAnimator = animator;
        ninjaVisual = visual;
    }

    public bool IsGrounded()
    {
        if (playerCollider == null) return false;
        float distance = playerCollider.bounds.extents.y + 0.2f;
        return Physics.Raycast(transform.position, Vector3.down, distance, ~0, QueryTriggerInteraction.Ignore);
    }

    void Update()
    {
        if (transform.position.y < -12f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        if (ninjaAnimator != null)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;
            ninjaAnimator.SetFloat("Speed", Mathf.Clamp01(velocity.magnitude / 2.5f));
            ninjaAnimator.SetBool("isGrounded", IsGrounded());

            if (ninjaVisual != null && velocity.sqrMagnitude > 0.15f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
                ninjaVisual.rotation = Quaternion.Slerp(ninjaVisual.rotation, targetRotation, Time.deltaTime * 12f);
            }
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (!value.isPressed || !IsGrounded()) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        if (audioSource != null && jumpClip != null) audioSource.PlayOneShot(jumpClip, 0.65f);
    }

    void FixedUpdate()
    {
        Vector3 direction;
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 forward = cam.transform.forward;
            Vector3 right = cam.transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            direction = right * moveInput.x + forward * moveInput.y;
        }
        else direction = new Vector3(moveInput.x, 0f, moveInput.y);
        if (direction.sqrMagnitude > 1f) direction.Normalize();
        rb.AddForce(direction * force, ForceMode.Acceleration);
        Vector3 horizontal = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float maxSpeed = level != null ? 5f : 25f;
        if (horizontal.magnitude > maxSpeed)
        {
            Vector3 clamped = horizontal.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(clamped.x, rb.linearVelocity.y, clamped.z);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            coins++;
            if (level != null) level.CollectCoin();
            AudioClip clip = GetComponent<Health>() != null ? GetComponent<Health>().pickupClip : null;
            if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
        }

        if (other.CompareTag("Exit"))
        {
            if (level != null) level.TryExit();
            else if (exitAnimator != null && exitAnimator.GetBool("Open")) Debug.Log("You win!");
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Health health = GetComponent<Health>();
            if (health != null) health.TakeDamage(1);
        }
    }
}
