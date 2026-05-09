using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("Görüþ Ayarlarý")]
    [SerializeField] private float viewRadius = 8f;       // Ne kadar uzaðý görebilir
    [SerializeField] private float viewAngle = 90f;       // Görüþ açýsý (derece)
    [SerializeField] private string targetTag = "Player"; // Kimi arýyor? (Enemy için "Player")

    [Header("Engel")]
    [SerializeField] private LayerMask obstacleMask;      // Duvar layer'ý (Inspector'dan seç)

    public bool CanSeeTarget { get; private set; }
    public Transform VisibleTarget { get; private set; }

    void Update()
    {
        FindVisibleTargets();
    }

    void FindVisibleTargets()
    {
        CanSeeTarget = false;
        VisibleTarget = null;

        // Tüm hedef tag'li objeleri bul
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject target in targets)
        {
            if (target == gameObject) continue; // Kendini ignore et

            Transform targetTransform = target.transform;
            Vector2 dirToTarget = (targetTransform.position - transform.position).normalized;
            float distToTarget = Vector2.Distance(transform.position, targetTransform.position);

            // 1. Mesafe kontrolü
            if (distToTarget > viewRadius) continue;

            // 2. Açý kontrolü (önümüzde mi?)
            float angleToTarget = Vector2.Angle(transform.right, dirToTarget); // Saða baktýðýný varsay (2D sprite flip)
            // Eðer sprite sola/saða dönüyorsa, transform.right kullanmak yerine bakýþ yönünü hesaplayalým:
            Vector2 facingDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            angleToTarget = Vector2.Angle(facingDir, dirToTarget);

            if (angleToTarget > viewAngle / 2f) continue;

            // 3. Engel kontrolü (Arada duvar var mý?)
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask);
            if (hit.collider != null) continue; // Duvar var, göremez

            // HEDEF GÖRÜLDÜ!
            CanSeeTarget = true;
            VisibleTarget = targetTransform;

            // Debug çizgisi (Scene view'da yeþil)
            Debug.DrawLine(transform.position, targetTransform.position, Color.green);
            break; // Ýlk bulduðunu al
        }
    }

    // Gizmos'ta görüþ alanýný göster
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector2 facingDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 rightBoundary = RotateVector(facingDir, viewAngle / 2f);
        Vector2 leftBoundary = RotateVector(facingDir, -viewAngle / 2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(rightBoundary * viewRadius));
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(leftBoundary * viewRadius));
    }

    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }
}