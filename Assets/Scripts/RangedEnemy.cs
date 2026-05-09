using UnityEngine;
using System.Collections.Generic;

public class RangedEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private float healthRegenRate = 5f;
    [SerializeField] private float regenDelay = 2f;

    [Header("Ateş Ayarları")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1.2f;
    [SerializeField] private float attackRange = 8f;

    [Header("Cover / Kaçma")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float lowHealthThreshold = 0.5f;   // %50 = Kaç
    [SerializeField] private float playerTooCloseRange = 3f;   // Player bu kadar yaklaşırsa kaç!

    private int currentHealth;
    private float fireTimer;
    private float lastDamageTime;
    private Transform player;
    private Rigidbody2D rb;

    private bool isHiding = false;
    private Vector2 hidePosition;
    private bool hasCoverTarget = false;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (firePoint == null)
            firePoint = transform;

        lastDamageTime = -999f;
    }

    void Update()
    {
        if (player == null) return;

        fireTimer -= Time.deltaTime;
        float healthPercent = (float)currentHealth / maxHealth;

        // 🏥 CAN YENİLEME (Saklanıyorsa)
        if (isHiding && Time.time > lastDamageTime + regenDelay && currentHealth < maxHealth)
        {
            currentHealth += Mathf.RoundToInt(healthRegenRate * Time.deltaTime);
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }

        // 🧠 AI KARARLARI
        if (healthPercent <= lowHealthThreshold && !isHiding)
        {
            GoToCover();
        }
        else if (isHiding && healthPercent >= 0.75f)
        {
            isHiding = false;
            hasCoverTarget = false;
        }

        // 🏃 PLAYER ÇOK YAKLAŞTI MI? KAÇ!
        if (isHiding && hasCoverTarget)
        {
            float distToHidePos = Vector2.Distance(player.position, hidePosition);
            if (distToHidePos < playerTooCloseRange)
            {
                // Player cover'a yaklaştı! Başka cover'a kaç!
                GoToDifferentCover();
            }
        }

        FacePlayer();
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;

        // 🏃 SAKLANMA MODU
        if (isHiding && hasCoverTarget)
        {
            float dist = Vector2.Distance(transform.position, hidePosition);

            if (dist > 0.3f)
            {
                Vector2 dir = (hidePosition - (Vector2)transform.position).normalized;
                rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
            return;
        }

        // 🔫 NORMAL SAVAŞ
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange && fireTimer <= 0f && PlayerCanSeeMe())
        {
            Shoot();
            fireTimer = fireRate;
        }

        if (distance < 4f)
        {
            Vector2 backDir = (transform.position - player.position).normalized;
            rb.MovePosition(rb.position + backDir * moveSpeed * Time.fixedDeltaTime);
        }
        else if (distance > 6f)
        {
            Vector2 approachDir = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + approachDir * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // 🏃 EN YAKIN COVER'A KAÇ
    void GoToCover()
    {
        GameObject[] covers = GameObject.FindGameObjectsWithTag("Cover");

        if (covers.Length == 0)
        {
            Vector2 fleeDir = ((Vector2)transform.position - (Vector2)player.position).normalized;
            hidePosition = (Vector2)transform.position + fleeDir * 5f;
            isHiding = true;
            hasCoverTarget = true;
            return;
        }

        Transform nearest = covers[0].transform;
        float nearestDist = Vector2.Distance(transform.position, nearest.position);

        foreach (GameObject cover in covers)
        {
            float d = Vector2.Distance(transform.position, cover.transform.position);
            if (d < nearestDist)
            {
                nearestDist = d;
                nearest = cover.transform;
            }
        }

        hidePosition = nearest.position;
        isHiding = true;
        hasCoverTarget = true;
    }

    // 🏃 BAŞKA COVER'A KAÇ (Mevcuttan farklı!)
    void GoToDifferentCover()
    {
        GameObject[] covers = GameObject.FindGameObjectsWithTag("Cover");

        if (covers.Length <= 1)
        {
            // Tek cover varsa, Player'dan uzaklaş
            Vector2 fleeDir = ((Vector2)transform.position - (Vector2)player.position).normalized;
            hidePosition = (Vector2)transform.position + fleeDir * 5f;
            return;
        }

        // Şu anki cover'dan en uzak olanı bul
        Transform farthest = null;
        float farDist = 0f;

        foreach (GameObject cover in covers)
        {
            // Şu anki cover'ı atla
            if (Vector2.Distance(cover.transform.position, hidePosition) < 0.5f)
                continue;

            float d = Vector2.Distance(player.position, cover.transform.position);
            if (d > farDist)
            {
                farDist = d;
                farthest = cover.transform;
            }
        }

        if (farthest != null)
        {
            hidePosition = farthest.position;
            Debug.Log("Player yaklaştı, başka cover'a kaçıyor!");
        }
        else
        {
            // Başka cover yoksa geri çekil
            Vector2 fleeDir = ((Vector2)transform.position - (Vector2)player.position).normalized;
            hidePosition = (Vector2)transform.position + fleeDir * 5f;
        }
    }

    // 👁️ Player beni görebiliyor mu?
    bool PlayerCanSeeMe()
    {
        Vector2 dir = (transform.position - player.position).normalized;
        float dist = Vector2.Distance(player.position, transform.position);

        RaycastHit2D[] hits = Physics2D.RaycastAll(player.position, dir, dist);

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject == gameObject) continue;
            if (hit.collider.transform.IsChildOf(transform)) continue;

            if (hit.collider.CompareTag("Wall"))
                return false;
        }

        return true;
    }

    void Shoot()
    {
        Vector2 shootDir = (player.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        EnemyProjectile proj = bullet.GetComponent<EnemyProjectile>();
        if (proj != null)
            proj.Setup(shootDir);
    }

    void FacePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        if (dir.x > 0.01f)
            transform.localScale = new Vector3(0.7f, 0.7f, 1);
        else if (dir.x < -0.01f)
            transform.localScale = new Vector3(-0.7f, 0.7f, 1);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        lastDamageTime = Time.time;

        Debug.Log("RangedEnemy hasar aldı! Kalan: " + currentHealth);

        StartCoroutine(HitFlash());

        if ((float)currentHealth / maxHealth <= lowHealthThreshold && !isHiding)
        {
            GoToCover();
        }

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

    void Die()
    {
        Destroy(gameObject);
    }
}