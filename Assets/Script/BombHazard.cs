
using UnityEngine;

public class BombHazard : Hazard
{
    public GameObject explosionPrefab;

    private bool fuseStarted = false;

    protected override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            base.OnCollisionEnter(other);
            return;
        }

        if (!fuseStarted)
        {
            fuseStarted = true;
            Debug.Log("Bomb landed!");
            Invoke(nameof(Explode), 2f);
        }
    }

    protected override void OnHitPlayer(Health player)
    {
        player.TakeDamage(2);
    }

    void Explode()
    {
        Debug.Log("Bomb exploded!");

        if (explosionPrefab != null)
        {
            GameObject effect = Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );

            Destroy(effect, 1f);
        }

        Destroy(gameObject);
    }
}
