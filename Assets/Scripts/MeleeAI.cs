using UnityEngine;

public class MeleeAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 1.3f;

    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Menzile girene kadar koþ
        if (distance > stopDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

            // Saða/sola bak
            if (direction.x > 0)
                transform.localScale = new Vector3(0.8f, 0.8f, 1);
            else if (direction.x < 0)
                transform.localScale = new Vector3(-0.8f, 0.8f, 1);
        }
    }
}