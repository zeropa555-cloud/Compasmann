using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;
    private SpriteRenderer sr;
    private Color originalColor;

    [Header("Animasyon")]
    public Animator anim;                    // 🆕 Düşmanın Animator'u
    public string hitTrigger = "Hit";        // 🆕 Trigger adı

    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;

        // 🆕 Animator'ı bul
        if (anim == null) anim = GetComponent<Animator>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // 🎬 HASAR ALINCA HIT ANİMASYONU OYNAT
        if (anim != null)
            anim.SetTrigger(hitTrigger);

        // Kısa beyaz flash (animasyonun yanında)
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
        if (sr != null) sr.color = originalColor;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}