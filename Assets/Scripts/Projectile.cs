using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 10;

    public Vector2 MoveDirection { get; private set; }  // 🆚 PUBLIC GETTER!
    private float lifeTimer;

    public void Setup(Vector2 direction)
    {
        MoveDirection = direction.normalized;
        lifeTimer = lifeTime;
    }

    void Update()
    {
        transform.position += (Vector3)(MoveDirection * speed * Time.deltaTime);
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") || other.CompareTag("EnemyBullet"))
            return;
        if (other.CompareTag("Player"))
            return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        RangedEnemy rangedEnemy = other.GetComponent<RangedEnemy>();
        if (rangedEnemy != null)
        {
            rangedEnemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}