using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 4f;
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
        // Kendi mermilerine çarpmasýn
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyBullet"))
            return;

        // Player'a hasar ver
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
                playerStats.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}