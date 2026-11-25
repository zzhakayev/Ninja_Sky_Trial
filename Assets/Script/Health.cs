using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int maxHP = 3;
    public AudioClip hurtClip;
    public AudioClip pickupClip;

    public int CurrentHP { get; private set; }
    private bool invincible;

    void Start()
    {
        CurrentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (invincible || CurrentHP <= 0) return;
        CurrentHP = Mathf.Max(0, CurrentHP - damage);
        GameAudio.Play("Hit", 0.85f);
        if (CurrentHP <= 0)
        {
            GameAudio.PlayFailure();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
        StartCoroutine(Invincibility());
    }

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(maxHP, CurrentHP + amount);
        GameAudio.Play("Pickup", 0.8f);
    }

    IEnumerator Invincibility()
    {
        invincible = true;
        List<Renderer> renderers = new List<Renderer>();
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            if (r.enabled) renderers.Add(r);
        }
        for (int i = 0; i < 8; i++)
        {
            foreach (Renderer r in renderers) if (r != null) r.enabled = i % 2 == 0;
            yield return new WaitForSeconds(0.15f);
        }
        foreach (Renderer r in renderers) if (r != null) r.enabled = true;
        invincible = false;
    }
}
