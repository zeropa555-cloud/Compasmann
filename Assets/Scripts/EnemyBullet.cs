using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 8f;
    public float damage = 10f;
    public float lifeTime = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 dir)
    {
        rb.linearVelocity = dir * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kendi mermisi veya düþmaný geç
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyBullet")) return;

        // Player'a hasar ver
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}