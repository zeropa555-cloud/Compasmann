using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 10;

    private Vector2 moveDirection;
    private float lifeTimer;

    public void Setup(Vector2 direction)
    {
        moveDirection = direction.normalized;
        lifeTimer = lifeTime;
    }

    void Update()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kendi mermilerine çarpmasın
        if (other.CompareTag("Bullet") || other.CompareTag("EnemyBullet"))
            return;

        // Player'a çarpmasın
        if (other.CompareTag("Player"))
            return;

        // 🎯 MELEE DÜŞMANA hasar ver (Enemy scripti)
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // 🎯 RANGED DÜŞMANA hasar ver (RangedEnemy scripti)
        RangedEnemy rangedEnemy = other.GetComponent<RangedEnemy>();
        if (rangedEnemy != null)
        {
            rangedEnemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}