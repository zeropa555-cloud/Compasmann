using UnityEngine;

public class EnemyWeaponAim : MonoBehaviour
{
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Player'a doðru açý
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Ters dönme fix (yukarý/aþaðý ters görünmemesi için)
        Vector3 localScale = transform.localScale;
        if (Mathf.Abs(angle) > 90f)
            localScale.y = -0.12f;
        else
            localScale.y = 0.12f;
        transform.localScale = localScale;
    }
}