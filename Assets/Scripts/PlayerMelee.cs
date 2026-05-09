using UnityEngine;
using System.Collections;

public class PlayerMelee : MonoBehaviour
{
    [Header("Melee Alan Hasarý")]
    public float attackRange = 1.8f;
    public float attackDamage = 30f;
    public float attackCooldown = 0.4f;
    public LayerMask enemyLayer;

    private bool canAttack = true;

    void Update()
    {
        // SOL CLICK = Etrafýna hasar (sadece bu item aktifken çalýþýr)
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(PerformMelee());
        }
    }

    IEnumerator PerformMelee()
    {
        canAttack = false;

        // Etrafýndaki düþmanlara hasar
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
                health.TakeDamage(attackDamage);
        }

        if (hitEnemies.Length > 0)
            Debug.Log("Item vurdu: " + hitEnemies.Length + " düþman");

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}