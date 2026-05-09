using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Idle Nefes Animasyonu")]
    public float bobAmount = 0.08f;      // Yukar�-a�a�� mesafe (0.05-0.1 aras�)
    public float bobSpeed = 3f;           // H�z (2-4 aras� ideal)

    private Vector3 localStartPos;
    private float timer;
    private Rigidbody2D parentRb;

    void Awake()
    {
        localStartPos = transform.localPosition;
        parentRb = GetComponentInParent<Rigidbody2D>();
    }

    void Update()
    {
        // Parent (Player) hareket etmiyorsa idle oynat
        if (parentRb != null && parentRb.linearVelocity.magnitude < 0.15f)
        {
            timer += Time.deltaTime;
            float yOffset = Mathf.Sin(timer * bobSpeed) * bobAmount;
            transform.localPosition = localStartPos + new Vector3(0, yOffset, 0);
        }
        else
        {
            // Hareket ederken yava��a ba�lang�� pozisyonuna d�n
            timer = 0;
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                localStartPos,
                Time.deltaTime * 10f
            );
        }
    }
}