using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.25f;
    
    [Header("Ses Ayarlari")]
    public AudioSource sesKaynagi;
    public AudioClip lazerSesi;

    private float lastFireTime;

    void Update()
    {
        // Sol Click basıldığında ve ateş etme süresi dolduğunda çalışır
        if (Input.GetMouseButtonDown(0) && Time.time >= lastFireTime + fireRate)
        {
            Shoot();
            lastFireTime = Time.time;
        }
    }

    void Shoot()
    {
        // Ses çalma komutu
        if (sesKaynagi != null && lazerSesi != null)
        {
            sesKaynagi.PlayOneShot(lazerSesi);
        }

        // Mermi oluşturma komutu
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null) 
        {
            b.SetDirection(firePoint.right);
        }
    }
}