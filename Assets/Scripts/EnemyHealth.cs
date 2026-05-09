using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;
    private SpriteRenderer sr;
    private Color originalColor;

    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();

        // Orijinal rengi kaydet (kýrmýzý placeholder)
        if (sr != null)
            originalColor = sr.color;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // BEYAZ FLASH (anlýk)
        if (sr != null)
        {
            sr.color = Color.white;
            Invoke(nameof(ResetColor), 0.1f);
        }

        if (currentHealth <= 0)
            Die();
    }

    void ResetColor()
    {
        // Kendi orijinal rengine dön (KIRMIZI)
        if (sr != null)
            sr.color = originalColor;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}