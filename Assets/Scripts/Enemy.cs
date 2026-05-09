using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("🩸 LIFESTEAL")]
    [SerializeField] private float lifestealPercent = 0.5f;

    [Header("Hareket")]
    [SerializeField] private float baseMoveSpeed = 3f;
    [SerializeField] private float rageMoveSpeed = 5.5f;
    [SerializeField] private float stopDistance = 1.3f;

    [Header("😡 RAGE")]
    [SerializeField] private float rageThreshold = 0.2f;    // %20 = RAGE!

    [Header("💨 Mermi Kaçınma")]
    [SerializeField] private float dodgeRange = 2.5f;       // Kaçınma mesafesi
    [SerializeField] private float dodgeForce = 12f;        // Kaçınma hızı
    [SerializeField] private float dodgeCooldown = 0.4f;    // Kaçınma cooldown
    [SerializeField] private float dodgeThreshold = 0.5f;   // %50 can = Kaçınma başlar

    private int currentHealth;
    private float attackTimer;
    private float dodgeTimer;
    private Transform player;
    private bool isDead = false;
    private bool isRaging = false;
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
        dodgeTimer = 0f;
    }

    void Update()
    {
        if (isDead || player == null) return;
        attackTimer -= Time.deltaTime;
        dodgeTimer -= Time.deltaTime;

        if (fov != null && !fov.CanSeeTarget) return;

        // 💨 Mermi kaçınma kontrolü (%50 altındaysa)
        if (currentHealth <= maxHealth * dodgeThreshold && dodgeTimer <= 0f)
        {
            CheckForBulletsAndDodge();
        }

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
        float currentSpeed = isRaging ? rageMoveSpeed : baseMoveSpeed;

        if (distance > stopDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * currentSpeed * Time.fixedDeltaTime);

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

    // 💨 MERMİ KAÇINMA
    void CheckForBulletsAndDodge()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, dodgeRange);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Bullet")) continue;

            Projectile proj = hit.GetComponent<Projectile>();
            if (proj == null) continue;

            // Mermi bize doğru mu geliyor?
            Vector2 toBullet = (hit.transform.position - transform.position).normalized;
            float dot = Vector2.Dot(toBullet, proj.MoveDirection);

            // dot < 0 ise mermi bize doğru geliyor (ters yönde)
            if (dot < -0.2f)
            {
                // 🎲 1/3 şans (33%)
                if (Random.value <= 0.33f)
                {
                    PerformDodge(proj.MoveDirection);
                    dodgeTimer = dodgeCooldown;
                }
                break; // Bir mermiden kaçınmaya çalış yeter
            }
        }
    }

    void PerformDodge(Vector2 bulletDir)
    {
        // Merminin geldiği yöne dik olarak kaç (sağa veya sola)
        Vector2 dodgeDir = Vector2.Perpendicular(bulletDir).normalized;

        // Rastgele sağ veya sol
        if (Random.value > 0.5f) dodgeDir = -dodgeDir;

        rb.AddForce(dodgeDir * dodgeForce, ForceMode2D.Impulse);

        Debug.Log("💨 MELEE Enemy mermiden KAÇTI!");
    }

    void AttackPlayer()
    {
        if (player == null) return;

        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);

            int healAmount = Mathf.RoundToInt(damage * lifestealPercent);
            currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log("🩸 MELEE hasar aldı! " + currentHealth + "/" + maxHealth);

        // 😡 RAGE: %20 altına düşünce
        if (!isRaging && (float)currentHealth / maxHealth <= rageThreshold)
        {
            isRaging = true;
            Debug.Log("😡 MELEE RAGE! Hız: " + baseMoveSpeed + " → " + rageMoveSpeed);
        }

        StartCoroutine(HitFlash());
        if (currentHealth <= 0) Die();
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

    void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Kaçınma alanı
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, dodgeRange);
    }
}