using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 20f;
    public float lifeTime = 2f;

    [Header("Mermi Trail (Ghost)")]
    public bool useTrail = true;
    public float trailDelay = 0.03f;
    public float trailFadeTime = 0.2f;
    public Color trailColor = new Color(1f, 0.8f, 0.2f, 0.6f);

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isAlive = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // KONTROL: Sprite var mı?
        if (sr == null)
        {
            Debug.LogError("Kanka! Bullet'ta SpriteRenderer yok!");
            return;
        }
        if (sr.sprite == null)
        {
            Debug.LogError("Kanka! Bullet SpriteRenderer'da sprite yok! Inspector'dan sprite at.");
        }

        Destroy(gameObject, lifeTime);

        if (useTrail && sr != null && sr.sprite != null)
            StartCoroutine(SpawnTrail());
    }

    public void SetDirection(Vector2 dir)
    {
        rb.linearVelocity = dir * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null) health.TakeDamage(damage);

            isAlive = false;
            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            isAlive = false;
            Destroy(gameObject);
        }
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
        ghost.transform.localScale = transform.localScale; // Aynı scale, küçültme yok

        SpriteRenderer ghostSr = ghost.AddComponent<SpriteRenderer>();
        ghostSr.sprite = sr.sprite;
        ghostSr.sortingLayerID = sr.sortingLayerID;
        ghostSr.sortingOrder = sr.sortingOrder; // -1 yok, aynı order (görünür olsun)
        ghostSr.color = trailColor;

        Debug.Log("Ghost oluştu! Pos: " + transform.position); // Test için

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
            Destroy(ghost);
    }
}