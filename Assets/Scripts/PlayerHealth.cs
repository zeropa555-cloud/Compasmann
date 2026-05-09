using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Can")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthSlider;

    [Header("Yenilmezlik (i-frames)")]
    public float invincibilityDuration = 0.5f;
    public bool isInvincible { get; private set; }

    [Header("Visual")]
    public Color normalColor = Color.white;
    public Color hitColor = Color.red;
    private SpriteRenderer sr;

    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();

        if (healthSlider != null)
            healthSlider.maxValue = maxHealth;
    }

    void Start()
    {
        UpdateHealthUI();
    }

    void Update()
    {
        // TEST: H tuþuna basýnca kendine 10 hasar ver
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthUI();
        StartCoroutine(InvincibilityFrames());

        // Ölüm kontrolü
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        // Kýrmýzý yanýp sönme efekti
        if (sr != null)
        {
            sr.color = hitColor;
            yield return new WaitForSeconds(0.1f);
            sr.color = normalColor;
        }

        // Yenilmezlik süresi
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("Kanka öldün! Game Over...");

        // Basit respawn: Pozisyonu sýfýrla, caný fulle
        // Ýleride sahne yeniden yüklenebilir veya Game Over ekraný gelir
        transform.position = Vector3.zero;
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Ghost trail'i durdurmak için (eðer hala dash'te kaldýysa)
        isInvincible = false;
    }
}