using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 40;
    [SerializeField] private float healthRegenRate = 3f;
    [SerializeField] private float regenDelay = 2f;

    [Header("Ateş")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float attackRange = 7f;

    [Header("Cover / Kaçma")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float lowHealthThreshold = 0.5f;  // %50 = Kaç
    [SerializeField] private float fleeRange = 2.5f;           // Player bu kadar yaklaşırsa başka cover'a git

    private int currentHealth;
    private float fireTimer;
    private float lastDamageTime;
    private float coverSearchTimer;    // Cover'a ulaşamazsa flee yap
    private Transform player;
    private Rigidbody2D rb;

    private bool isHiding = false;
    private Transform currentCover = null;
    private bool isStuck = false;      // Duvara takıldı mı?

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        player = GameObject.FindWithTag("Player")?.transform;
        if (firePoint == null) firePoint = transform;

        lastDamageTime = -999f;
        coverSearchTimer = 0f;
    }

    void Update()
    {
        if (player == null) return;
        fireTimer -= Time.deltaTime;

        float healthPercent = (float)currentHealth / maxHealth;

        // 🏥 Can yenileme (Saklanıyorsa)
        if (isHiding && Time.time > lastDamageTime + regenDelay && currentHealth < maxHealth)
        {
            currentHealth += Mathf.RoundToInt(healthRegenRate * Time.deltaTime);
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }

        // 🧠 AI Kararları
        if (healthPercent <= lowHealthThreshold && !isHiding)
        {
            FindNearestCover();
            coverSearchTimer = 0f;
        }

        // Player cover'a çok yaklaştı mı? Başka cover'a kaç!
        if (isHiding && currentCover != null)
        {
            float distToCover = Vector2.Distance(player.position, currentCover.position);
            if (distToCover < fleeRange)
            {
                FindDifferentCover();
                coverSearchTimer = 0f;
            }
        }

        // Can dolduysa dön
        if (isHiding && healthPercent >= 0.8f)
        {
            isHiding = false;
            currentCover = null;
            isStuck = false;
            rb.linearVelocity = Vector2.zero;
        }

        // ⏰ 3 saniyedir cover'a ulaşamadıysa FLEE yap (takılmıştır)
        if (isHiding && !isStuck)
        {
            coverSearchTimer += Time.deltaTime;
            if (coverSearchTimer > 3f)
            {
                isStuck = true;
                currentCover = null; // Cover'ı unut, sadece kaç
            }
        }

        FacePlayer();
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;

        // 🏃 FLEE MODU (Takıldıysa sadece Player'dan uzaklaş)
        if (isHiding && isStuck)
        {
            Vector2 fleeDir = ((Vector2)transform.position - (Vector2)player.position).normalized;
            rb.linearVelocity = fleeDir * moveSpeed;
            return;
        }

        // 🏃 COVER'A GİT
        if (isHiding && currentCover != null)
        {
            Vector2 dir = ((Vector2)currentCover.position - (Vector2)transform.position).normalized;
            float dist = Vector2.Distance(transform.position, currentCover.position);

            if (dist > 0.3f)
            {
                rb.linearVelocity = dir * moveSpeed;
            }
            else
            {
                // Vardı!
                rb.linearVelocity = Vector2.zero;
                coverSearchTimer = 0f;
            }
            return;
        }

        // 🔫 NORMAL SAVAŞ
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

    // 🧱 DUVARA ÇARPIRSA YÖN DEĞİŞTİR (Sağa/Sola kay)
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Wall")) return;

        if (isHiding && currentCover != null && !isStuck)
        {
            // Duvara çarpınca 90° sağa veya sola dön
            Vector2 toCover = ((Vector2)currentCover.position - (Vector2)transform.position).normalized;
            Vector2 avoidDir = Vector2.Perpendicular(toCover);

            // Hangi yön daha iyi? (Cover'a daha yakın olanı seç)
            Vector2 pos = transform.position;
            float distRight = Vector2.Distance(pos + avoidDir, currentCover.position);
            float distLeft = Vector2.Distance(pos - avoidDir, currentCover.position);

            if (distRight < distLeft)
                rb.linearVelocity = avoidDir * moveSpeed;
            else
                rb.linearVelocity = -avoidDir * moveSpeed;

            // 0.4 saniye sonra tekrar cover'a yönel
            Invoke(nameof(RealignToCover), 0.4f);
        }
    }

    void RealignToCover()
    {
        if (currentCover != null && !isStuck)
        {
            Vector2 dir = ((Vector2)currentCover.position - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * moveSpeed;
        }
    }

    void FindNearestCover()
    {
        GameObject[] covers = GameObject.FindGameObjectsWithTag("Cover");
        if (covers.Length == 0)
        {
            isStuck = true; // Cover yoksa direk flee
            return;
        }

        Transform best = covers[0].transform;
        float bestDist = Vector2.Distance(transform.position, best.position);

        foreach (var c in covers)
        {
            float d = Vector2.Distance(transform.position, c.transform.position);
            if (d < bestDist) { bestDist = d; best = c.transform; }
        }

        currentCover = best;
        isHiding = true;
        isStuck = false;
    }

    void FindDifferentCover()
    {
        GameObject[] covers = GameObject.FindGameObjectsWithTag("Cover");
        if (covers.Length <= 1) { isStuck = true; return; }

        Transform best = null;
        float bestDist = 0f;

        foreach (var c in covers)
        {
            if (c.transform == currentCover) continue;

            float d = Vector2.Distance(player.position, c.transform.position);
            if (d > bestDist) { bestDist = d; best = c.transform; }
        }

        if (best != null)
        {
            currentCover = best;
            coverSearchTimer = 0f;
        }
        else
        {
            isStuck = true;
        }
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
        currentHealth -= dmg;
        lastDamageTime = Time.time;
        StartCoroutine(HitFlash());

        if ((float)currentHealth / maxHealth <= lowHealthThreshold && !isHiding)
        {
            FindNearestCover();
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