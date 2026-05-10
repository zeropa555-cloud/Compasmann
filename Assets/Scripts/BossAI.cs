using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    [Header("Hedef")]
    public Transform player;

    [Header("Hareket")]
    public float moveSpeed = 2f;
    public float stopDistance = 8f;       // Ateş mesafesi

    [Header("Faz 1 - Normal Saldırı")]
    public GameObject topPrefab;          // Top prefabı (BossTop)
    public float attackCooldown = 3f;       // Kaç saniyede bir atsın
    public float chargeTime = 0.5f;         // Hazırlanma
    public float fireOffset = 0.5f;         // Boss'tan ne kadar uzakta çıksın

    [Header("Faz 2 - Wave Saldırı (Can <= 100)")]
    public float phase2Threshold = 100f;
    public GameObject warningZonePrefab;  // Kırmızı zone prefabı
    public int waveCount = 10;              // 10 tane wave
    public float waveInterval = 0.5f;       // 0.5 saniye arayla
    public float waveLifetime = 1.5f;       // Zone ne kadar kalsın

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private BossHealth bossHealth;
    private bool isAttacking = false;
    private bool phase2Done = false;        // Bir kere girsin
    private float lastAttackTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        bossHealth = GetComponent<BossHealth>();

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (topPrefab == null) Debug.LogError("❌ topPrefab BOŞ! Inspector'dan ata!");
        if (warningZonePrefab == null) Debug.LogWarning("⚠️ warningZonePrefab boş!");
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        // 🎯 FAZ 2 KONTROL: Can 100'ün altına düştü mü?
        if (!phase2Done && bossHealth != null && bossHealth.currentHealth <= phase2Threshold)
        {
            StartCoroutine(Phase2WaveAttack());
        }

        // Yön çevirme
        float dirX = player.position.x - transform.position.x;
        if (dirX > 0.1f) sr.flipX = false;
        else if (dirX < -0.1f) sr.flipX = true;
    }

    void FixedUpdate()
    {
        if (player == null || isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // 🎯 Faz 1: Yakınsa dur ve ateş et
        if (distance <= stopDistance && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(NormalAttack());
        }
        else if (distance > stopDistance)
        {
            // Uzaksa yaklaş
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * moveSpeed;
        }
        else
        {
            // Yakınsa ama cooldown varsa dur
            rb.linearVelocity = Vector2.zero;
        }
    }

    // 🎬 FAZ 1: Normal Saldırı (tek top atar)
    IEnumerator NormalAttack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(chargeTime);
        ShootTop();

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        lastAttackTime = Time.time;
    }

    // 🔥 FAZ 2: 10 Wave Saldırı (hasar almaz!)
    IEnumerator Phase2WaveAttack()
    {
        phase2Done = true;
        isAttacking = true;           // Normal saldırıyı durdur
        rb.linearVelocity = Vector2.zero;

        Debug.Log("🔥🔥🔥 FAZ 2 BAŞLADI! 10 wave + hasar almaz!");

        // 🛡️ Hasar almayı kapat
        if (bossHealth != null) bossHealth.SetInvincible(true);

        // 🌊 10 TANE ZONE (0.5 saniye arayla)
        for (int i = 0; i < waveCount; i++)
        {
            SpawnWarningZone();
            yield return new WaitForSeconds(waveInterval);
        }

        yield return new WaitForSeconds(1f);

        // 🛡️ Hasar almayı tekrar aç
        if (bossHealth != null) bossHealth.SetInvincible(false);
        isAttacking = false;          // Tekrar normal saldırmaya devam et
        lastAttackTime = Time.time;   // Cooldown sıfırla

        Debug.Log("⚔️ FAZ 2 BİTTİ! Tekrar hasar alabilir ve top atar.");
    }

    void SpawnWarningZone()
    {
        if (warningZonePrefab == null) return;

        // Player etrafında rastgele pozisyon
        Vector2 randomOffset = Random.insideUnitCircle * 5f;
        Vector2 spawnPos = (Vector2)player.position + randomOffset;

        GameObject zone = Instantiate(warningZonePrefab, spawnPos, Quaternion.identity);

        // Zone'a top prefab'ını ve player'ı ver
        BossWarningZone wz = zone.GetComponent<BossWarningZone>();
        if (wz != null)
        {
            wz.topPrefab = topPrefab;
            wz.targetPlayer = player;
        }

        Destroy(zone, waveLifetime);
    }

    void ShootTop()
    {
        if (topPrefab == null) return;

        // Player yönü
        Vector2 dir = (player.position - transform.position).normalized;

        // Boss'tan ileride spawn et
        Vector3 spawnPos = transform.position + (Vector3)(dir * fireOffset);

        // Açı hesapla (2D dönüş)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 🎯 OLUŞTUR
        GameObject top = Instantiate(topPrefab, spawnPos, Quaternion.Euler(0, 0, angle));

        // 🚀 HAREKET ETTİR (mermi gibi serbest!)
        BossTop bt = top.GetComponent<BossTop>();
        if (bt != null)
        {
            bt.SetDirection(dir);
        }
        else
        {
            // Yedek
            Rigidbody2D topRb = top.GetComponent<Rigidbody2D>();
            if (topRb != null)
            {
                topRb.gravityScale = 0f;
                topRb.linearVelocity = dir * 10f;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}