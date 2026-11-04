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
    private AudioSource audioSource;

    void Start()
    {
        CurrentHP = maxHP;
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        if (invincible || CurrentHP <= 0) return;
        CurrentHP = Mathf.Max(0, CurrentHP - damage);
        if (audioSource != null && hurtClip != null) audioSource.PlayOneShot(hurtClip);
        if (CurrentHP <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
        StartCoroutine(Invincibility());
    }

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(maxHP, CurrentHP + amount);
        if (audioSource != null && pickupClip != null) audioSource.PlayOneShot(pickupClip);
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
