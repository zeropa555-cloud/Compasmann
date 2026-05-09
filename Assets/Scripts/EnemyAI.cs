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
    public float weaponRotation = -90f;

    [Header("Oda Sınırları (Duvarlar)")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    // 🎬 SADECE AWAKE'DE ANIMATOR BAŞLAT, SONRA DOKUNMA!
    private Animator anim;
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

        // 🎬 Animator'ı bul ve Walk'ı başlat (doğduğu anda koşsun)
        anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.Play("Walk"); // 🆕 Doğar doğmaz koşmaya başlar
        }

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
            rb.linearVelocity = Vector2.zero; // Durur ama animasyon devam eder!

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

        // DUVAR SINIRLARI
        if (useBounds)
        {
            Vector2 pos = rb.position;
            pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
            pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
            rb.position = pos;
        }

        // 🎬 HİÇBİR ANIMATOR KONTROLÜ YOK! (Sadece tek state, sürekli loop)
    }

    IEnumerator PerformAttack()
    {
        if (swordWavePrefab != null && swordSpawnPoint != null)
        {
            GameObject wave = Instantiate(
                swordWavePrefab,
                swordSpawnPoint.position,
                swordSpawnPoint.rotation
            );

            if (!facingRight)
            {
                Vector3 scale = wave.transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                wave.transform.localScale = scale;
            }

            wave.transform.SetParent(swordSpawnPoint);
            Destroy(wave, waveDuration);
        }

        yield return new WaitForSeconds(0.2f);

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

        if (useBounds)
        {
            Gizmos.color = Color.blue;
            Vector2 center = (minBounds + maxBounds) / 2f;
            Vector2 size = maxBounds - minBounds;
            Gizmos.DrawWireCube(new Vector3(center.x, center.y, 0), new Vector3(size.x, size.y, 0));
        }
    }
}