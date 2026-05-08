using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 1f;

    private int currentHealth;
    private float attackTimer;
    private Transform player;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Player'� bul
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (isDead || player == null) return;

        attackTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        // Yak�nsa vur
        if (distance <= attackRange && attackTimer <= 0f)
        {
            AttackPlayer();
            attackTimer = attackCooldown;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator HitFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color original = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sr.color = original;
    }

    void AttackPlayer()
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats != null)
            playerStats.TakeDamage(damage);
    }

    void Die()
    {
        isDead = true;

        // Eski FindObjectOfType yerine yeni ve hızlı olan FindFirstObjectByType kullanıyoruz
        RoomManager room = Object.FindFirstObjectByType<RoomManager>();
        
        if (room != null)
        {
            room.EnemyDied();
        }

        Destroy(gameObject); // Düşmanı yok et
    }
}
