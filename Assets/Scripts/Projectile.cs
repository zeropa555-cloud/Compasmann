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
        // Kendi mermilerine çarpmasýn
        if (other.CompareTag("Bullet"))
            return;

        // Player'a çarpmasýn (mermi Player'ýn içinden çýkýyor olabilir)
        if (other.CompareTag("Player"))
            return;

        // Düþmana hasar ver
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}