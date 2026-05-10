using UnityEngine;
using System.Collections;

public class EnemyShooter : MonoBehaviour
{
    [Header("Takip")]
    public Transform player;
    public float moveSpeed = 2f;
    public float stopDistance = 5f;
    public float retreatDistance = 3f;

    [Header("Silah & Ateş")]
    public GameObject bulletPrefab;
    public Transform weaponVisual;
    public Transform firePoint;
    public float fireRate = 1.5f;

    [Header("Silah Pozisyonu")]
    public Vector3 weaponRightPos = new Vector3(0.4f, 0, 0);
    public Vector3 weaponLeftPos = new Vector3(-0.4f, 0, 0);

    [Header("Animasyon")]
    public Animator anim;
    public string walkState = "ShooterWalk";  // 🆕 "ShooterWalk" oldu

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private SpriteRenderer weaponSr;
    private float lastFireTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        anim = GetComponent<Animator>();
        if (anim != null) anim.Play(walkState);

        if (weaponVisual != null)
            weaponSr = weaponVisual.GetComponent<SpriteRenderer>();

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector2 dir = player.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (dir.x > 0.1f)
        {
            if (sr != null) sr.flipX = false;
        }
        else if (dir.x < -0.1f)
        {
            if (sr != null) sr.flipX = true;
        }

        if (weaponVisual != null)
        {
            weaponVisual.localPosition = dir.x >= 0 ? weaponRightPos : weaponLeftPos;
            weaponVisual.rotation = Quaternion.Euler(0, 0, angle);
            if (weaponSr != null) weaponSr.flipY = dir.x < 0;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < retreatDistance)
        {
            Vector2 awayDir = (transform.position - player.position).normalized;
            rb.linearVelocity = awayDir * moveSpeed;
        }
        else if (distance <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;

            if (Time.time >= lastFireTime + fireRate)
            {
                Shoot();
                lastFireTime = Time.time;
            }
        }
        else
        {
            Vector2 moveDir = (player.position - transform.position).normalized;
            rb.linearVelocity = moveDir * moveSpeed;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, weaponVisual.rotation);
        Vector2 shootDir = (player.position - firePoint.position).normalized;

        EnemyBullet eb = bullet.GetComponent<EnemyBullet>();
        if (eb != null) eb.SetDirection(shootDir);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }
}