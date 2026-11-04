
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public AudioClip shootClip;
    public float cooldown = 0.3f;

    private float nextShot;
    private Vector3 shootDirection = Vector3.forward;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnMove(InputValue value)
    {
        Vector2 move = value.Get<Vector2>();

        Vector3 direction = new Vector3(
            move.x,
            0f,
            move.y
        );

        if (direction.magnitude > 0.1f)
        {
            shootDirection = direction.normalized;
        }
    }

    public void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (Time.time < nextShot || bulletPrefab == null)
            return;

        nextShot = Time.time + cooldown;

        Vector3 spawnPosition =
            transform.position + shootDirection * 0.9f;

        Instantiate(
            bulletPrefab,
            spawnPosition,
            Quaternion.LookRotation(shootDirection)
        );

        if (audioSource != null && shootClip != null)
        {
            audioSource.PlayOneShot(shootClip);
        }
    }
}
    