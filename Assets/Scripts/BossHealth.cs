using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 200f;
    public float currentHealth;

    private SpriteRenderer sr;
    private Color originalColor;
    private bool isInvincible = false;

    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= amount;

        if (sr != null)
        {
            sr.color = Color.white;
            Invoke(nameof(ResetColor), 0.1f);
        }

        if (currentHealth <= 0) Die();
    }

    void ResetColor()
    {
        if (sr != null && !isInvincible) sr.color = originalColor;
    }

    // 🆕 HASAR ALMAYI AÇ/KAPAT (Phase 2 için)
    public void SetInvincible(bool value)
    {
        isInvincible = value;
        if (sr != null) sr.color = value ? Color.gray : originalColor;
        Debug.Log(value ? "🛡️ Boss hasar alamaz!" : "⚔️ Boss hasar alabilir!");
    }

    void Die()
    {
        Debug.Log("💀 BOSS ÖLDÜ!");
        Destroy(gameObject);
    }
}