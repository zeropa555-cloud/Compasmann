using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float damage = 15f;
    public float damageCooldown = 1f;
    private float lastDamageTime;

    void OnTriggerEnter2D(Collider2D other)
    {
        // 🔍 DEBUG: Neye değdiğini gör
        Debug.Log("Kılıç değdi: " + other.name + " | Tag: " + other.tag);

        if (other.CompareTag("Player"))
        {
            if (Time.time >= lastDamageTime + damageCooldown)
            {
                PlayerHealth health = other.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    lastDamageTime = Time.time;
                    Debug.Log("✅ Player'a hasar verildi: " + damage);
                }
                else
                {
                    Debug.LogError("❌ Player'da PlayerHealth YOK!");
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