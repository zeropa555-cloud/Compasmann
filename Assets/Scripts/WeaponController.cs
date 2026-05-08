using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Silahlar")]
    [SerializeField] private GameObject gunObject;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Gun Ayarları")]
    [SerializeField] private float fireRate = 0.2f;

    private float fireTimer;
    private Camera mainCamera;
    private bool isGunActive = true;

    // 🆕 Dışarıdan okunabilir property (MeleeAttack bunu kontrol edecek)
    public bool IsGunActive => isGunActive;

    void Start()
    {
        mainCamera = Camera.main;
        if (firePoint == null)
            firePoint = transform;

        UpdateWeaponVisibility();
    }

    void Update()
    {
        // 🔫 1 tuşu = Gun modu
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isGunActive = true;
            UpdateWeaponVisibility();
        }

        // ⚔️ 2 tuşu = Melee modu
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isGunActive = false;
            UpdateWeaponVisibility();
        }

        // 🔫 Sol tık (LMB) = Sadece Gun modunda ateş eder
        if (isGunActive)
        {
            fireTimer -= Time.deltaTime;
            if (Input.GetMouseButton(0) && fireTimer <= 0f)
            {
                Shoot();
                fireTimer = fireRate;
            }
        }
    }

    void UpdateWeaponVisibility()
    {
        if (gunObject != null)
            gunObject.SetActive(isGunActive);
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

        if (CinemachineShake.Instance != null)
            CinemachineShake.Instance.Shake(0.08f, 1.5f);
    }
}