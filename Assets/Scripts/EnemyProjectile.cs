using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private int damage = 10;

    private Vector2 moveDirection;
    private float lifeTimer;

    private RangedEnemy owner;
    private int ownerDamage;
    private float ownerLifesteal;
    private bool hasLifesteal = false;

    public void Setup(Vector2 direction)
    {
        moveDirection = direction.normalized;
        lifeTimer = lifeTime;
    }

    public void SetOwner(RangedEnemy enemy, int dmg, float lifesteal)
    {
        owner = enemy;
        ownerDamage = dmg;
        ownerLifesteal = lifesteal;
        hasLifesteal = true;
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
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyBullet"))
            return;

        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);

                if (hasLifesteal && owner != null)
                {
                    owner.OnProjectileHitPlayer(damage);
                }
            }
        }

        Destroy(gameObject);
    }
}