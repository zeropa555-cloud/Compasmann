using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Ateþ Ayarlarý")]
    public GameObject bulletPrefab;
    public Transform firePoint;  // RangedWeapon/FirePoint
    public float fireRate = 0.25f;

    private float lastFireTime;

    void Update()
    {
        // SOL CLICK = Ateþ (sadece RangedWeapon aktifken çalýþýr)
        if (Input.GetMouseButtonDown(0) && Time.time >= lastFireTime + fireRate)
        {
            Shoot();
            lastFireTime = Time.time;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
            b.SetDirection(firePoint.right);
    }
}