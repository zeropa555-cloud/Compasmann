using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private Camera cam;
    private Transform player;
    private PlayerMovement playerMovement;
    private SpriteRenderer playerSr;
    private SpriteRenderer sr;

    [Header("Pozisyon")]
    public float distanceFromPlayer = 0.5f;

    void Awake()
    {
        cam = Camera.main;
        player = transform.parent;
        playerMovement = GetComponentInParent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();

        playerSr = player.GetComponent<SpriteRenderer>();
        if (playerSr == null) playerSr = player.GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (cam == null || player == null) return;

        // 1. MOUSE YÖNÜNE DÖNME
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - player.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 2. DAÝRESEL POZÝSYON (içine girmesin!)
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * distanceFromPlayer;
        transform.localPosition = offset;

        // 3. KARAKTER SAÐA/SOLA DÖNME (A/D ile)
        if (playerSr != null) playerSr.flipX = !playerMovement.facingRight;

        // 4. SÝLAH FLIP
        if (sr != null) sr.flipY = (lookDir.x < 0);
    }
}