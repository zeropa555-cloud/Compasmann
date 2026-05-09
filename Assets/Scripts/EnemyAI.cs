using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Takip")]
    public Transform player;
    public float moveSpeed = 3f;

    [Header("Saldiri")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.2f;
    public float attackDamage = 15f;
    public LayerMask playerLayer;

    [Header("Kılıç Dalga (Animasyon)")]
    public GameObject swordWavePrefab;
    public Transform swordSpawnPoint;
    public float waveDuration = 0.4f;

    [Header("Kılıç Yönü")]
    public Transform weaponVisual;
    public Vector3 weaponRightPos = new Vector3(0.4f, 0, 0);
    public Vector3 weaponLeftPos = new Vector3(-0.4f, 0, 0);

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool facingRight = true;
    private float lastAttackTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float dirX = player.position.x - transform.position.x;
        if (dirX > 0.1f) facingRight = true;
        else if (dirX < -0.1f) facingRight = false;

        if (sr != null) sr.flipX = !facingRight;

        if (weaponVisual != null)
        {
            weaponVisual.localPosition = facingRight ? weaponRightPos : weaponLeftPos;

            SpriteRenderer weaponSr = weaponVisual.GetComponent<SpriteRenderer>();
            if (weaponSr != null)
                weaponSr.flipX = !facingRight;
        }

        if (swordSpawnPoint != null)
            swordSpawnPoint.localPosition = facingRight ? weaponRightPos : weaponLeftPos;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(PerformAttack());
                lastAttackTime = Time.time;
            }
        }
        else
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * moveSpeed;
        }
    }

    IEnumerator PerformAttack()
    {
        if (swordWavePrefab != null && swordSpawnPoint != null)
        {
            // 🔍 Debug: Nerede spawn edeceğini görelim
            Debug.Log("Spawn pozisyon: " + swordSpawnPoint.position);

            // 🎬 Prefab'ı SWORDSPAWN pozisyonunda ve rotasyonunda oluştur
            GameObject wave = Instantiate(
                swordWavePrefab,
                swordSpawnPoint.position,
                swordSpawnPoint.rotation
            );

            // 🆕 POZİSYONU EXPLICIT OLARAK AYARLA (garanti olsun)
            wave.transform.position = swordSpawnPoint.position;
            wave.transform.rotation = swordSpawnPoint.rotation;

            // 🆕 PARENT YAP - World pozisyonunu KORU (true = korur)
            wave.transform.SetParent(swordSpawnPoint, true);

            // 🔄 Sola bakıyorsa ters çevir
            if (!facingRight)
            {
                Vector3 scale = wave.transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                wave.transform.localScale = scale;
            }

            // 🆕 Eğer hâlâ görünmüyorsa, SpriteRenderer'ı kontrol et
            SpriteRenderer waveSr = wave.GetComponent<SpriteRenderer>();
            if (waveSr != null)
            {
                Debug.Log("Wave sprite: " + waveSr.sprite);
                waveSr.sortingOrder = 10; // Öne çıksın
            }

            Destroy(wave, waveDuration);
        }

        yield return new WaitForSeconds(0.2f);

        // Hasar verme kısmı aynı...
        Vector2 attackDir = facingRight ? Vector2.right : Vector2.left;
        Vector2 attackCenter = (Vector2)transform.position + attackDir * 0.5f;

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackCenter, 0.6f, playerLayer);

        foreach (Collider2D col in hitPlayers)
        {
            PlayerHealth health = col.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(attackCooldown - 0.2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        float dir = facingRight ? 1f : -1f;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, new Vector3(dir * attackRange, 0, 0));
    }
}