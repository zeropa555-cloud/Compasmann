using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Takip")]
    public Transform player;
    public float moveSpeed = 3f;

    [Header("Kılıç Yönü")]
    public Transform weaponVisual;       // EnemyWeapon objesi
    public Vector3 weaponRightPos = new Vector3(0.3f, 0, 0);
    public Vector3 weaponLeftPos = new Vector3(-0.3f, 0, 0);
    public float weaponRightRot = -90f;  // Sağa bakınca Z açısı
    public float weaponLeftRot = 90f;    // Sola bakınca Z açısı

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool facingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Player'ı otomatik bul
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Yön belirle (sağa mı sola mı)
        float dirX = player.position.x - transform.position.x;
        if (dirX > 0.1f) facingRight = true;
        else if (dirX < -0.1f) facingRight = false;

        // Düşman sprite'ını çevir
        if (sr != null) sr.flipX = !facingRight;

        // 🗡️ KILIÇ POZİSYONU + DÖNÜŞÜ
        if (weaponVisual != null)
        {
            if (facingRight)
            {
                // SAĞA BAKIYOR
                weaponVisual.localPosition = weaponRightPos;
                weaponVisual.localRotation = Quaternion.Euler(0, 0, weaponRightRot);
            }
            else
            {
                // SOLA BAKIYOR
                weaponVisual.localPosition = weaponLeftPos;
                weaponVisual.localRotation = Quaternion.Euler(0, 0, weaponLeftRot);
            }
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // HER ZAMAN TAKİP ET (hiç durma)
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }
}