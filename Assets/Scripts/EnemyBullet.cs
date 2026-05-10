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

    // 🆕 FİZİKSEL ÇARPIŞMA (IsTrigger = false)
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject target = collision.gameObject;

        // 🎯 ROOM'A ÇARPIINCA: GEÇ, YOK OLMA! (Room IsTrigger=true olduğu için zaten çarpmaz ama garanti)
        if (target.CompareTag("Room") || target.CompareTag("Ground"))
        {
            return;
        }

        // Düşman ve kendi mermisi geç
        if (target.CompareTag("Enemy") || target.CompareTag("EnemyBullet")) return;

        // Player'a hasar ver
        if (target.CompareTag("Player"))
        {
            PlayerHealth health = target.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(damage);
        }

        // Duvar veya herhangi bir şeye çarpınca yok ol
        Destroy(gameObject);
    }
}