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
    private SpriteRenderer sr;
    private Color normalColor;

    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) normalColor = sr.color;

        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        if (healthSlider != null) healthSlider.maxValue = maxHealth;
    }

    void Start()
    {
        UpdateHealthUI();
    }

    void Update()
    {
        // 🎮 TEST: H tuşuna basınca kendine 10 hasar ver
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);
            Debug.Log("TEST: Hasar alindi! Animasyon calisiyor mu?");
        }
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthUI();

        // 🎬 Hasar alinca trigger calisir
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

        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = normalColor;
        }

        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("Kanka oldun!");
        transform.position = Vector3.zero;
        currentHealth = maxHealth;
        UpdateHealthUI();
        isInvincible = false;
    }
}