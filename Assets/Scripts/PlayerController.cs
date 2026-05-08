using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    [SerializeField] private float moveSpeed = 6f;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        // Gravity olmasýn, dönmeyi kilitle (fizik karýþmasýn)
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // WASD input al (GetAxisRaw = anýnda durur, kaygan his vermez)
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        // Çapraz giderken hýzlanmasýn di normalize et
        movementInput = movementInput.normalized;

        // Mouse'a bak
        LookAtMouse();
    }

    void FixedUpdate()
    {
        // Fizik ile hareket (daha smooth ve çarpýþmalarda düzgün çalýþýr)
        rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
    }

    void LookAtMouse()
    {
        // Mouse pozisyonunu dünya koordinatýna çevir
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        // Oyuncudan mouse'a doðru vektör
        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        // Saða mý sola mý bakýyor? (Soul Knight stili flip)
        if (direction.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);   // Saða bak
        else if (direction.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);  // Sola bak (ters çevir)
    }
}