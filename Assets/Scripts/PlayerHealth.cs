using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider;
    public float invincibilityDuration = 0.5f;
    public bool isInvincible { get; private set; }

    private Animator anim;

    void Awake()
    {
        currentHealth = maxHealth;

        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        if (healthSlider != null) healthSlider.maxValue = maxHealth;
    }

    void Start() { UpdateHealthUI(); }

    public void TakeDamage(float amount)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthUI();

        // 🎬 Hasar alinca HIT animasyonu (kirmizi yok, sadece animasyon)
        if (anim != null) anim.SetTrigger("Hit");

        StartCoroutine(InvincibilityFrames());
        if (currentHealth <= 0) Die();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null) healthSlider.value = currentHealth;
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        // Sadece bekle, renk degisimi YOK
        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("Oldun!");
        transform.position = Vector3.zero;
        currentHealth = maxHealth;
        UpdateHealthUI();
        isInvincible = false;
    }
}