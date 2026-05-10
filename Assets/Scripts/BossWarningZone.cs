using UnityEngine;
using System.Collections;

public class BossWarningZone : MonoBehaviour
{
    public GameObject topPrefab;      // Düşecek top
    public Transform targetPlayer;  // Hedef
    public float damage = 20f;
    public float warnTime = 0.6f;   // Uyarı süresi

    private bool playerInside = false;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // 🚨 YARI SAYDAM KIRMIZI UYARI
        if (sr != null) sr.color = new Color(1f, 0.2f, 0.2f, 0.5f);

        StartCoroutine(ZoneSequence());
    }

    IEnumerator ZoneSequence()
    {
        // ⏳ Uyarı süresi (parla, kıpırdama)
        yield return new WaitForSeconds(warnTime);

        // 💥 TOP DÜŞÜR
        if (topPrefab != null)
        {
            GameObject top = Instantiate(topPrefab, transform.position, Quaternion.identity);

            BossTop bt = top.GetComponent<BossTop>();
            if (bt != null)
            {
                // Aşağı doğru veya player'a doğru
                Vector2 dir = targetPlayer != null ?
                    ((Vector2)targetPlayer.position - (Vector2)transform.position).normalized :
                    Vector2.down;

                bt.SetDirection(dir);
            }
        }

        // 🎯 PLAYER HÂLÂ ÜZERİNDEYSE HASAR
        if (playerInside)
        {
            PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
        }

        // Beyaz flash (patlama hissi)
        if (sr != null) sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInside = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInside = false;
    }
}