using UnityEngine;
using System.Collections;

public class PlayerMelee : MonoBehaviour
{
    [Header("Saldiri Alani (Hasar Veren)")]
    public float attackRange = 1.8f;
    public float attackDamage = 30f;
    public float attackCooldown = 0.6f;
    public LayerMask enemyLayer;

    [Header("Miknatis Dalga (Sadece Animasyon)")]
    public GameObject wavePrefab;
    public Transform waveSpawnPoint;
    public float waveDuration = 0.4f;

    [Header("Yön & Pozisyon")]
    public Vector3 rightPos = new Vector3(0.4f, 0, 0);
    public Vector3 leftPos = new Vector3(-0.4f, 0, 0);

    private bool canAttack = true;
    private PlayerMovement playerMovement;
    private SpriteRenderer sr;

    void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (playerMovement == null) return;

        // SAĞA BAKIYOR (D) → Z = -90
        if (playerMovement.facingRight)
        {
            transform.localPosition = rightPos;
            transform.localRotation = Quaternion.Euler(0, 0, -90f);
            if (sr != null) sr.flipX = false;
        }
        // SOLA BAKIYOR (A) → Z = +90
        else
        {
            transform.localPosition = leftPos;
            transform.localRotation = Quaternion.Euler(0, 0, 90f);
            if (sr != null) sr.flipX = true;
        }

        // SOL CLICK = Saldiri
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;

        // 🎬 1. MIKNATIS DALGASI SPAWN
        if (wavePrefab != null && waveSpawnPoint != null)
        {
            // Prefab'ın kendi rotasyonuyla oluştur (senin açın kalır)
            GameObject wave = Instantiate(wavePrefab, waveSpawnPoint.position, wavePrefab.transform.rotation);

            // 🆕 PARENT YAP: Karakterle beraber hareket etsin, arkada kalmasın!
            wave.transform.SetParent(waveSpawnPoint);

            // 🔄 SOLA BAKIYORSA: Ters çevir (scale.x = -1)
            if (!playerMovement.facingRight)
            {
                Vector3 scale = wave.transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                wave.transform.localScale = scale;
            }

            // Animasyon bitince yok ol
            Destroy(wave, waveDuration);
        }

        // ⏱️ 2. BEKLE (dalganın ortasında)
        yield return new WaitForSeconds(0.2f);

        // 💥 3. HASAR VER (mevcut melee alani)
        Vector2 attackCenter = (Vector2)transform.position;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackCenter, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null) health.TakeDamage(attackDamage);
        }

        if (hitEnemies.Length > 0)
            Debug.Log("Vurdu: " + hitEnemies.Length + " dusman");

        // ⏱️ 4. KALAN COOLDOWN BEKLE (dalga bitsin + bekleme)
        yield return new WaitForSeconds(attackCooldown - 0.2f);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        if (playerMovement == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}