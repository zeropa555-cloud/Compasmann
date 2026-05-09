using UnityEngine;
using System.Collections;

public class PlayerMelee : MonoBehaviour
{
    [Header("Tıklama Vuruşu")]
    public float attackRange = 1.8f;
    public float attackDamage = 30f;
    public float attackCooldown = 0.4f;
    public LayerMask enemyLayer;

    [Header("Yön & Pozisyon")]
    public float forwardOffset = 0.5f;
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

        // SOL CLICK = Hasar
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(PerformMelee());
        }
    }

    IEnumerator PerformMelee()
    {
        canAttack = false;

        Vector2 offset = Vector2.zero;
        if (playerMovement != null)
        {
            offset = playerMovement.facingRight
                ? Vector2.right * forwardOffset
                : Vector2.left * forwardOffset;
        }

        Vector2 attackCenter = (Vector2)transform.position + offset;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackCenter, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null) health.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        if (playerMovement == null) return;
        float offset = playerMovement.facingRight ? forwardOffset : -forwardOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.right * offset, attackRange);
    }
}