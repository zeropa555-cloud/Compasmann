using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 20f;
    public float lifeTime = 2f;

    [Header("Mermi Trail")]
    public bool useTrail = true;
    public float trailDelay = 0.03f;
    public float trailFadeTime = 0.2f;
    public Color trailColor = new Color(1f, 0.8f, 0.2f, 0.6f);

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isAlive = true;
    private List<GameObject> activeGhosts = new List<GameObject>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (rb != null)
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Destroy(gameObject, lifeTime);

        if (useTrail && sr != null && sr.sprite != null)
            StartCoroutine(SpawnTrail());
    }

    public void SetDirection(Vector2 dir)
    {
        if (rb != null)
            rb.linearVelocity = dir * speed;
    }

    // 🆕 FİZİKSEL ÇARPIŞMA
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject target = collision.gameObject;

        // 🔍 Debug log
        Debug.Log("🔴 Çarptı: " + target.name + " | Tag: " + target.tag);

        // 🆕 EĞER KILIÇSA (veya child obje), PARENT'INA BAK!
        if (!target.CompareTag("Enemy") && target.transform.parent != null)
        {
            target = target.transform.parent.gameObject;
            Debug.Log("🔄 Parent'a yönlendirildi: " + target.name + " | Parent Tag: " + target.tag);
        }

        HasarVer(target);
    }

    // 🆕 TRIGGER ÇARPIŞMA (yedek)
    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject target = other.gameObject;

        // 🆕 EĞER KILIÇSA, PARENT'INA BAK!
        if (!target.CompareTag("Enemy") && target.transform.parent != null)
        {
            target = target.transform.parent.gameObject;
        }

        HasarVer(target);
    }

    // 🆕 ORTAK HASAR VERME
    void HasarVer(GameObject target)
    {
        // Kendine veya Player'a çarpma
        if (target.CompareTag("Bullet") || target.CompareTag("Player")) return;

        // Düşmana çarpınca hasar ver
        if (target.CompareTag("Enemy"))
        {
            EnemyHealth health = target.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Debug.Log("✅ Hasar verildi: " + damage + " | Hedef: " + target.name);
            }
            else
            {
                Debug.LogError("❌ " + target.name + " üzerinde EnemyHealth YOK!");
            }
        }
        else
        {
            Debug.Log("ℹ️ Düşman değil, yok oluyor: " + target.name);
        }

        // Yok ol
        isAlive = false;
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        foreach (GameObject ghost in activeGhosts)
        {
            if (ghost != null) Destroy(ghost);
        }
        activeGhosts.Clear();
    }

    IEnumerator SpawnTrail()
    {
        while (isAlive && gameObject != null)
        {
            CreateGhost();
            yield return new WaitForSeconds(trailDelay);
        }
    }

    void CreateGhost()
    {
        if (sr == null || sr.sprite == null) return;

        GameObject ghost = new GameObject("BulletGhost");
        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;
        ghost.transform.localScale = transform.localScale;

        SpriteRenderer ghostSr = ghost.AddComponent<SpriteRenderer>();
        ghostSr.sprite = sr.sprite;
        ghostSr.sortingLayerID = sr.sortingLayerID;
        ghostSr.sortingOrder = sr.sortingOrder;
        ghostSr.color = trailColor;

        activeGhosts.Add(ghost);
        StartCoroutine(FadeGhost(ghost, ghostSr));
    }

    IEnumerator FadeGhost(GameObject ghost, SpriteRenderer ghostSr)
    {
        float elapsed = 0f;
        Color startColor = ghostSr.color;

        while (elapsed < trailFadeTime && ghost != null)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / trailFadeTime);
            ghostSr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        if (ghost != null)
        {
            activeGhosts.Remove(ghost);
            Destroy(ghost);
        }
    }
}