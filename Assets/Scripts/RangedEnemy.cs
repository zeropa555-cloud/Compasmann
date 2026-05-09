using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 40;

    [Header("❤️ GÖRÜŞ DIŞI CAN YENİLEME")]
    [SerializeField] private float healthRegenRate = 5f;     // Saniyede 5 can
    [SerializeField] private float regenDelay = 2f;          // Hasar alınca 2 sn bekle
    [SerializeField] private bool showRegenLog = true;       // Console'da göster

    [Header("Ateş")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float attackRange = 7f;

    [Header("Kaçma")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float lowHealthThreshold = 0.5f;
    [SerializeField] private float fleeDistance = 8f;
    [SerializeField] private float zigzagInterval = 0.4f;

    private int currentHealth;
    private float fireTimer;
    private float lastDamageTime;
    private Transform player;
    private Rigidbody2D rb;
    private FieldOfView fov;

    private bool isFleeing = false;
    private float zigzagTimer = 0f;
    private int zigzagSide = 1;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        player = GameObject.FindWithTag("Player")?.transform;
        if (firePoint == null) firePoint = transform;

        lastDamageTime = -999f;
        fov = GetComponent<FieldOfView>();
    }

    void Update()
    {
        if (player == null) return;
        fireTimer -= Time.deltaTime;

        float healthPercent = (float)currentHealth / maxHealth;

        // 🚫👁️ GÖRÜŞ DIŞINDAYSA CAN YENİLE! (Senin onu göremediğin/göremediği yerde)
        bool isHidden = (fov == null) || !fov.CanSeeTarget;  // Player'ı göremiyorsa = Gizli
        bool canRegen = Time.time > lastDamageTime + regenDelay && currentHealth < maxHealth;

        if (canRegen && isHidden)
        {
            int prevHealth = currentHealth;
            currentHealth += Mathf.RoundToInt(healthRegenRate * Time.deltaTime);
            currentHealth = Mathf.Min(currentHealth, maxHealth);

            if (showRegenLog && currentHealth > prevHealth)
            {
                Debug.Log("❤️ GÖRÜŞ DIŞINDA! Enemy can yenileniyor: " + prevHealth + " → " + currentHealth);
            }
        }

        // Can azaldı mı? KAÇ!
        if (healthPercent <= lowHealthThreshold && !isFleeing)
        {
            if (fov == null || fov.CanSeeTarget)
            {
                isFleeing = true;
                zigzagTimer = 0f;
                zigzagSide = Random.value > 0.5f ? 1 : -1;
                Debug.Log("🏃 Enemy kaçıyor! Can: " + currentHealth);
            }
        }

        // Yeterince uzaklaştı mı ve can doldu mu? Dön
        if (isFleeing)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist >= fleeDistance && healthPercent >= 0.8f)
            {
                isFleeing = false;
                rb.linearVelocity = Vector2.zero;
                Debug.Log("✅ Enemy savaşa dönüyor! Can: " + currentHealth);
            }
        }

        FacePlayer();
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;

        // KAÇMA MODU (Zigzag)
        if (isFleeing)
        {
            Vector2 fleeDir = ((Vector2)transform.position - (Vector2)player.position).normalized;

            zigzagTimer += Time.fixedDeltaTime;
            if (zigzagTimer >= zigzagInterval)
            {
                zigzagTimer = 0f;
                zigzagSide *= -1;
            }

            float zigzagAngle = zigzagSide * 45f;
            Vector2 zigzagDir = RotateVector(fleeDir, zigzagAngle);

            rb.linearVelocity = zigzagDir * moveSpeed;
            return;
        }

        // 🚫 GÖREMİYORSA IDLE (Dur)
        if (fov != null && !fov.CanSeeTarget)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 🔫 NORMAL SAVAŞ (Görebiliyorsa)
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireRate;
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

    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    void Shoot()
    {
        Vector2 dir = (player.position - firePoint.position).normalized;
        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        EnemyProjectile p = b.GetComponent<EnemyProjectile>();
        if (p != null) p.Setup(dir);
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

        Debug.Log("🩸 Enemy hasar aldı! " + prevHealth + " → " + currentHealth);

        StartCoroutine(HitFlash());

        if ((float)currentHealth / maxHealth <= lowHealthThreshold && !isFleeing)
        {
            if (fov == null || fov.CanSeeTarget)
            {
                isFleeing = true;
                zigzagTimer = 0f;
                zigzagSide = Random.value > 0.5f ? 1 : -1;
            }
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
}