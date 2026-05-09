using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float damage = 15f;
    public float damageCooldown = 1f;
    private float lastDamageTime;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time >= lastDamageTime + damageCooldown)
            {
                PlayerHealth health = other.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    lastDamageTime = Time.time;
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time >= lastDamageTime + damageCooldown)
            {
                PlayerHealth health = other.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    lastDamageTime = Time.time;
                }
            }
        }
    }
}