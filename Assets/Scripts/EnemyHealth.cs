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
        if (sr != null) originalColor = sr.color;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // Hasar aninda BEYAZ FLASH (0.1sn)
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
        // Kendi orijinal rengine don (beyaz degil!)
        if (sr != null) sr.color = originalColor;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}