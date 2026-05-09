using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Düşman Stats")]
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Hareket")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 1.3f;

    private int currentHealth;
    private float attackTimer;
    private Transform player;
    private bool isDead = false;
    private Rigidbody2D rb;
    private FieldOfView fov;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        fov = GetComponent<FieldOfView>();
    }

    void Update()
    {
        if (isDead || player == null) return;
        attackTimer -= Time.deltaTime;

        if (fov != null && !fov.CanSeeTarget) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange && attackTimer <= 0f)
        {
            AttackPlayer();
            attackTimer = attackCooldown;
        }
    }

    void FixedUpdate()
    {
        if (isDead || player == null || rb == null) return;
        if (fov != null && !fov.CanSeeTarget)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > stopDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

            if (direction.x > 0.01f)
                transform.localScale = new Vector3(0.8f, 0.8f, 1);
            else if (direction.x < -0.01f)
                transform.localScale = new Vector3(-0.8f, 0.8f, 1);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log("🩸 MELEE Enemy hasar aldı! Kalan can: " + currentHealth + "/" + maxHealth + " (SABİT CAN — yenilenmez)");

        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
            Die();
    }

    System.Collections.IEnumerator HitFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;
        Color original = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sr.color = original;
    }

    void AttackPlayer()
    {
        if (player == null) return;
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats != null)
            playerStats.TakeDamage(damage);
    }

    void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}