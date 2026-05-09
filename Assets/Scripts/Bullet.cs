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
        // Düşmana carpinca hasar ver (RANGED)
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null) health.TakeDamage(damage);
            isAlive = false;
            Destroy(gameObject);
        }

        // Duvara carpinca yok ol
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            isAlive = false;
            Destroy(gameObject);
        }
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