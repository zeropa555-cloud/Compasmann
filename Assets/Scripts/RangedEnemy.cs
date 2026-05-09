using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 40;
    [SerializeField] private int damage = 10;
    [SerializeField] private float healthRegenRate = 5f;
    [SerializeField] private float regenDelay = 2f;

    [Header("🩸 LIFESTEAL")]
    [SerializeField] private float lifestealPercent = 0.3f;

    [Header("Ateş")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float baseFireRate = 1.5f;
    [SerializeField] private float rageFireRate = 0.4f;
    [SerializeField] private float attackRange = 7f;

    [Header("Hareket")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("😡 RAGE")]
    [SerializeField] private float rageThreshold = 0.2f;    // %20 = RAGE!

    [Header("💨 Mermi Kaçınma")]
    [SerializeField] private float dodgeRange = 2.5f;
    [SerializeField] private float dodgeForce = 12f;
    [SerializeField] private float dodgeCooldown = 0.4f;
    [SerializeField] private float dodgeThreshold = 0.5f;   // %50 can = Kaçınma

    private int currentHealth;
    private float fireTimer;
    private float dodgeTimer;
    private float lastDamageTime;
    private Transform player;
    private Rigidbody2D rb;
    private FieldOfView fov;
    private bool isRaging = false;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        player = GameObject.FindWithTag("Player")?.transform;
        if (firePoint == null) firePoint = transform;

        lastDamageTime = -999f;
        dodgeTimer = 0f;
        fov = GetComponent<FieldOfView>();
    }

    void Update()
    {
        if (player == null) return;
        fireTimer -= Time.deltaTime;
        dodgeTimer -= Time.deltaTime;

        float healthPercent = (float)currentHealth / maxHealth;

        // Görüş dışındaysa can yenile
        bool isHidden = (fov == null) || !fov.CanSeeTarget;
        bool canRegen = Time.time > lastDamageTime + regenDelay && currentHealth < maxHealth;

        if (canRegen && isHidden)
        {
            int prevHealth = currentHealth;
            currentHealth += Mathf.RoundToInt(healthRegenRate * Time.deltaTime);
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }

        // 💨 Mermi kaçınma kontrolü (%50 altındaysa)
        if (currentHealth <= maxHealth * dodgeThreshold && dodgeTimer <= 0f)
        {
            CheckForBulletsAndDodge();
        }

        FacePlayer();
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;
        if (fov != null && !fov.CanSeeTarget)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        float currentFireRate = isRaging ? rageFireRate : baseFireRate;

        if (distance <= attackRange && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = currentFireRate;
        }

        if (distance < 4f)
        {
            Vector2 backDir = (transform.position - player.position).normalized;
            rb.linearVelocity = backDir * moveSpeed;
        }
        else if (distance > 6f)
        {
            Vector2 approachDir = (player.position - transform.position).normalized;
            rb.linearVelocity = approachDir * moveSpeed;
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

            Vector2 toBullet = (hit.transform.position - transform.position).normalized;
            float dot = Vector2.Dot(toBullet, proj.MoveDirection);

            if (dot < -0.2f)
            {
                // 🎲 1/3 şans (33%)
                if (Random.value <= 0.33f)
                {
                    PerformDodge(proj.MoveDirection);
                    dodgeTimer = dodgeCooldown;
                }
                break;
            }
        }
    }

    void PerformDodge(Vector2 bulletDir)
    {
        Vector2 dodgeDir = Vector2.Perpendicular(bulletDir).normalized;
        if (Random.value > 0.5f) dodgeDir = -dodgeDir;

        rb.AddForce(dodgeDir * dodgeForce, ForceMode2D.Impulse);

        Debug.Log("💨 RANGED Enemy mermiden KAÇTI!");
    }

    void Shoot()
    {
        Vector2 dir = (player.position - firePoint.position).normalized;
        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        EnemyProjectile proj = b.GetComponent<EnemyProjectile>();
        if (proj != null)
        {
            proj.Setup(dir);
            proj.SetOwner(this, damage, lifestealPercent);
        }
    }

    public void OnProjectileHitPlayer(int damageDealt)
    {
        int healAmount = Mathf.RoundToInt(damageDealt * lifestealPercent);
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

        Debug.Log("🩸 RANGED can ÇEKTİ! +" + healAmount);
    }

    void FacePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        if (dir.x > 0) transform.localScale = new Vector3(0.7f, 0.7f, 1);
        else if (dir.x < 0) transform.localScale = new Vector3(-0.7f, 0.7f, 1);
    }

    public void TakeDamage(int dmg)
    {
        int prevHealth = currentHealth;
        currentHealth -= dmg;
        lastDamageTime = Time.time;

        Debug.Log("🩸 RANGED hasar aldı! " + prevHealth + " → " + currentHealth);

        // 😡 RAGE: %20 altına düşünce
        if (!isRaging && (float)currentHealth / maxHealth <= rageThreshold)
        {
            isRaging = true;
            Debug.Log("😡 RANGED RAGE! Ateş: " + baseFireRate + " → " + rageFireRate);
        }

        if (currentHealth <= 0)
        {
            rb.linearVelocity = Vector2.zero;
            Destroy(gameObject);
        }
    }

    System.Collections.IEnumerator HitFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;
        Color o = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sr.color = o;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, dodgeRange);
    }
}