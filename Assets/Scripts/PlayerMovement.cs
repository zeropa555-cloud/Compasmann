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

    [Header("Ses Ayarlari")]
    public AudioSource sesKaynagi;
    public AudioClip dashSesi;

    [Header("Ghost Trail")]
    public float ghostDelay = 0.03f;
    public float ghostFadeTime = 0.3f;
    public Color ghostColor = new Color(0.5f, 0.8f, 1f, 0.7f);

    [Header("Animasyon")]
    public string speedParam = "Speed";
    public string dashingParam = "IsDashing";

    [Header("Yön")]
    public bool facingRight = true;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private Vector2 moveInput;
    private Vector2 lastMoveDir = Vector2.down;
    private bool canDash = true;

    private Transform visualTransform;
    private Vector3 originalVisualScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // SpriteRenderer'ı bul
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();

        // Visual objenin transformunu ve orijinal scale'ini al
        if (sr != null)
        {
            visualTransform = sr.transform;
            originalVisualScale = visualTransform.localScale;
        }

        anim = GetComponent<Animator>();
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(h, v).normalized;

        // A/D ILE SAĞA/SOLA DÖNME
        if (h > 0) facingRight = true;
        else if (h < 0) facingRight = false;
        
        if (sr != null) sr.flipX = !facingRight;

        if (moveInput != Vector2.zero)
            lastMoveDir = moveInput;

        // ANIMATOR
        if (anim != null)
        {
            float currentSpeed = rb.linearVelocity.magnitude;
            anim.SetFloat(speedParam, currentSpeed);

            if (!string.IsNullOrEmpty(dashingParam))
                anim.SetBool(dashingParam, isDashing);
        }

        if (Input.GetKeyDown(KeyCode.Space) && canDash && !isDashing)
            StartCoroutine(Dash());
    }

    void FixedUpdate()
    {
        if (!isDashing)
            rb.linearVelocity = moveInput * moveSpeed;
    }

    void LateUpdate()
    {
        // Animasyondan sonra visual scale'i sabitle
        if (visualTransform != null && visualTransform.localScale != originalVisualScale)
            visualTransform.localScale = originalVisualScale;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        // DASH SESİNİ ÇALMA
        if (sesKaynagi != null && dashSesi != null)
        {
            sesKaynagi.PlayOneShot(dashSesi);
        }

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
        if (sr == null || sr.sprite == null) return;

        GameObject ghost = new GameObject("Ghost");
        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;
        ghost.transform.localScale = transform.localScale;

        SpriteRenderer ghostSr = ghost.AddComponent<SpriteRenderer>();
        ghostSr.sprite = sr.sprite;
        ghostSr.sortingLayerID = sr.sortingLayerID;
        ghostSr.sortingOrder = sr.sortingOrder;
        ghostSr.color = ghostColor;
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