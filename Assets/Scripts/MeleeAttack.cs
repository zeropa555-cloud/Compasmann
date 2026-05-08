using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("Alan Saldırı Ayarları")]
    [SerializeField] private int damage = 25;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float attackRange = 1.5f;

    private float attackTimer;
    private bool isAttacking = false;
    private WeaponController weaponController;

    void Start()
    {
        weaponController = GetComponent<WeaponController>();
    }

    void Update()
    {
        // 🛑 Eğer Gun modu aktifse (1 tuşu), sağ tık HIÇ ÇALIŞMASIN!
        if (weaponController != null && weaponController.IsGunActive)
            return;

        attackTimer -= Time.deltaTime;

        // Sağ tık (RMB) = Sadece Melee modunda çalışır (2 tuşu)
        if (Input.GetMouseButtonDown(1) && attackTimer <= 0f && !isAttacking)
        {
            PerformAttack();
            attackTimer = attackCooldown;
        }
    }

    void PerformAttack()
    {
        isAttacking = true;

        // Karakterin etrafındaki dairesel alanda düşmanları bul
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(damage);
            }
        }

        // Hafif geri tepme hissi
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 recoilDir = (transform.position - mousePos).normalized;
        transform.position += (Vector3)(recoilDir * 0.1f);

        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}