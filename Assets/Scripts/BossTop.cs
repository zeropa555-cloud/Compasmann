using UnityEngine;

public class BossTop : MonoBehaviour
{
    public float speed = 10f;         // 🆕 Hızlı olsun (mermi gibi)
    public float damage = 25f;
    public float lifeTime = 3f;

    private Rigidbody2D rb;
    private bool directionSet = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("❌ BossTop'ta Rigidbody2D YOK! Eklemelisin.");
            return;
        }

        rb.gravityScale = 0f;           // 🆕 Düşmesin!
        rb.freezeRotation = true;       // 🆕 Dönmesin (gerekirse)
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // 🆕 Takılmasın

        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 dir)
    {
        if (rb == null)
        {
            Debug.LogError("❌ SetDirection: rb NULL!");
            return;
        }

        // 🎯 KESİN HAREKET - Unity 6'da linearVelocity kullan!
        rb.linearVelocity = dir * speed;
        directionSet = true;

        Debug.Log("✅ BossTop hareket ediyor! Velocity: " + rb.linearVelocity);
    }

    void Update()
    {
        // 🆕 Eğer velocity 0 olduysa tekrar zorla (güvenlik)
        if (directionSet && rb != null && rb.linearVelocity.magnitude < 0.1f)
        {
            Debug.LogWarning("⚠️ Velocity düştü, tekrar başlatılıyor...");
            // Son bilinen yönü kullan (eğer varsa)
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss") || other.CompareTag("BossTop")) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Wall") || other.CompareTag("Room") || other.CompareTag("Ground"))
        {
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}