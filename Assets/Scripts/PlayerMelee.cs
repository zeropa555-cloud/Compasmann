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

        // 🎬 MIKNATIS DALGASI SPAWN
        if (wavePrefab != null && waveSpawnPoint != null)
        {
            GameObject wave = Instantiate(wavePrefab, waveSpawnPoint.position, wavePrefab.transform.rotation);
            wave.transform.SetParent(waveSpawnPoint);

            if (!playerMovement.facingRight)
            {
                Vector3 scale = wave.transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                wave.transform.localScale = scale;
            }

            Destroy(wave, waveDuration);
        }

        yield return new WaitForSeconds(0.2f);

        // 💥 HASAR VER (MELEE - alan içindeki düşmanlar)
        Vector2 attackCenter = (Vector2)transform.position;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackCenter, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null) health.TakeDamage(attackDamage);
        }

        if (hitEnemies.Length > 0)
            Debug.Log("Melee vurdu: " + hitEnemies.Length + " dusman");

        yield return new WaitForSeconds(attackCooldown - 0.2f);
        canAttack = true;
    }

    void OnDrawGizmos()
    {
        if (playerMovement != null)
        {
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.9f);
            Gizmos.DrawWireSphere(transform.position, attackRange);

            float dir = playerMovement.facingRight ? 1f : -1f;
            Vector3 forward = new Vector3(dir * attackRange, 0, 0);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, forward);
            Gizmos.DrawSphere(transform.position + forward, 0.08f);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}