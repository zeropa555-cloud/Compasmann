using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket")]
    public float moveSpeed = 6f;

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.8f;
    public bool isDashing { get; private set; }

    [Header("Ghost Trail")]
    public float ghostDelay = 0.03f;
    public float ghostFadeTime = 0.3f;
    public Color ghostColor = new Color(0.5f, 0.8f, 1f, 0.7f); // Daha belirgin alpha

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 moveInput;
    private Vector2 lastMoveDir = Vector2.down;
    private bool canDash = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Eğer sprite atanmamışsa uyar
        if (sr.sprite == null)
        {
            Debug.LogError("Kanka! Player'in SpriteRenderer'inda sprite yok. Inspector'dan sprite at.");
        }
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(h, v).normalized;

        if (moveInput != Vector2.zero)
            lastMoveDir = moveInput;

        if (Input.GetKeyDown(KeyCode.Space) && canDash && !isDashing)
            StartCoroutine(Dash());
    }

    void FixedUpdate()
    {
        if (!isDashing)
            rb.linearVelocity = moveInput * moveSpeed;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        Vector2 dashDir = (moveInput != Vector2.zero) ? moveInput : lastMoveDir;
        rb.linearVelocity = dashDir * dashSpeed;

        StartCoroutine(SpawnGhosts());

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator SpawnGhosts()
    {
        while (isDashing)
        {
            CreateGhost();
            yield return new WaitForSeconds(ghostDelay);
        }
    }

    void CreateGhost()
    {
        if (sr.sprite == null) return; // Sprite yoksa çık

        GameObject ghost = new GameObject("Ghost");
        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;
        ghost.transform.localScale = transform.localScale;

        SpriteRenderer ghostSr = ghost.AddComponent<SpriteRenderer>();
        ghostSr.sprite = sr.sprite;

        // Sorting Layer AYNI kalsın, order'ı Player ile AYNI yap (negatif olmasın)
        ghostSr.sortingLayerID = sr.sortingLayerID;
        ghostSr.sortingOrder = sr.sortingOrder; // -1 yerine aynı değerde

        // Başlangıç rengi (tam görünür)
        ghostSr.color = ghostColor;

        // Fizik etkileşimi olmasın
        ghost.layer = gameObject.layer;

        StartCoroutine(FadeGhost(ghost, ghostSr));
    }

    IEnumerator FadeGhost(GameObject ghost, SpriteRenderer ghostSr)
    {
        float elapsed = 0f;
        Color startColor = ghostSr.color;

        while (elapsed < ghostFadeTime && ghost != null)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / ghostFadeTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, t);
            ghostSr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        if (ghost != null)
            Destroy(ghost);
    }
}