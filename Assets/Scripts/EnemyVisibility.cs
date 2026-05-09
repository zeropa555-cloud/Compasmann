using UnityEngine;

public class EnemyVisibility : MonoBehaviour
{
    private Transform player;
    private SpriteRenderer sr;
    private Collider2D myCollider;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        
        sr = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (player == null || sr == null) return;

        // Player'dan bana doðru raycast
        Vector2 dir = (transform.position - player.position).normalized;
        float dist = Vector2.Distance(player.position, transform.position);
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(player.position, dir, dist);
        
        bool wallBetween = false;
        
        foreach (var hit in hits)
        {
            if (hit.collider == myCollider) continue;
            if (hit.collider.transform.IsChildOf(transform)) continue;
            if (transform.IsChildOf(hit.collider.transform)) continue;
                
            if (hit.collider.CompareTag("Wall"))
            {
                wallBetween = true;
                break;
            }
        }

        // Duvar varsa = Yarý saydam (soluk), yoksa = Tam görünür
        Color c = sr.color;
        c.a = wallBetween ? 0.25f : 1.0f;
        sr.color = c;
    }
}