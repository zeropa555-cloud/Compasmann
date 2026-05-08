using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Ateş Ayarları")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;

    private float fireTimer;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (firePoint == null)
            firePoint = transform;
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        if (Input.GetMouseButton(0) && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    void Shoot()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 shootDirection = (mousePos - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Projectile proj = bullet.GetComponent<Projectile>();
        if (proj != null)
            proj.Setup(shootDirection);

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 🎥 MAUS SOL TIK = CINEMACHINE TITREME (6D Shake)
        if (CinemachineShake.Instance != null)
            CinemachineShake.Instance.Shake(0.08f, 1.5f);
    }
}