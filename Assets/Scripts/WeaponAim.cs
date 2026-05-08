using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Mouse pozisyonu
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Silahtan mouse'a doðru açý
        Vector2 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Silahý döndür
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Silah ters dönmesin (yukarý/aþaðý doðru ters görünmemesi için flip)
        Vector3 localScale = transform.localScale;
        if (Mathf.Abs(angle) > 90f)
            localScale.y = -0.2f; // Ters çevir
        else
            localScale.y = 0.2f;  // Normal
        transform.localScale = localScale;
    }
}