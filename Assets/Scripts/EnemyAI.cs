using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Takip")]
    public Transform player;
    public float moveSpeed = 3f;
    public float stopDistance = 1.5f;   // Saldırı mesafesinde dur

    [Header("Saldiri (Kilic)")]
    public float attackRange = 1.8f;     // Saldırı menzili
    public float attackDamage = 15f;     // Verdiği hasar
    public float attackCooldown = 1.2f;  // Tekrar saldırı süresi
    public LayerMask playerLayer;         // Player layer'ı

    [Header("Görsel")]
    public SpriteRenderer sr;            // Düşmanın sprite'ı
    public Transform weaponVisual;         // EnemyWeapon objesi

    private Rigidbody2D rb;
    private bool canAttack = true;
    private bool facingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (sr == null) sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        // Yön belirle (sağa/sola bak)
        float dirX = player.position.x - transform.position.x;
        if (dirX > 0.1f) facingRight = true;
        else if (dirX < -0.1f) facingRight = false;

        // Sprite çevir
        if (sr != null) sr.flipX = !facingRight;

        // Silah pozisyonu (sağda/solda)
        if (weaponVisual != null)
        {
            weaponVisual.localPosition = facingRight
                ? new Vector3(0.3f, 0, 0)
                : new Vector3(-0.3f, 0, 0);
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Saldırı menzili içinde mi?
        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero; // Dur

            if (canAttack)
            {
                StartCoroutine(PerformAttack());
            }
        }
        // Uzaksa takip et
        else if (distance > stopDistance)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * moveSpeed;
        }
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;

        // 🎬 Saldırı animasyonu/başlangıcı (ilerde buraya animasyon trigger eklersin)
        Debug.Log("Düşman saldırdı!");

        // Hasar ver (ön taraftaki alanda)
        Vector2 attackDir = facingRight ? Vector2.right : Vector2.left;
        Vector2 attackCenter = (Vector2)transform.position + attackDir * 0.5f;

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackCenter, 0.6f, playerLayer);

        foreach (Collider2D col in hitPlayers)
        {
            PlayerHealth health = col.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        // Saldırı menzili (kırmızı)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Yön oku (yeşil)
        if (sr != null)
        {
            float dir = facingRight ? 1f : -1f;
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, new Vector3(dir * attackRange, 0, 0));
        }
    }
}