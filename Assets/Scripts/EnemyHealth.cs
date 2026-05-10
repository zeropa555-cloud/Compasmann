using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;
    private SpriteRenderer sr;
    private Color originalColor;

    [Header("Animasyon")]
    public Animator anim;
    public string hitTrigger = "Hit";

    private RoomManager myRoom;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
        if (anim == null) anim = GetComponent<Animator>();
    }

    // 🆕 RoomManager'dan çağrılır
    public void SetRoom(RoomManager room)
    {
        myRoom = room;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (anim != null) anim.SetTrigger(hitTrigger);
        if (sr != null)
        {
            sr.color = Color.white;
            Invoke(nameof(ResetColor), 0.1f);
        }

        if (currentHealth <= 0) Die();
    }

    void ResetColor()
    {
        if (sr != null) sr.color = originalColor;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // 🆕 ROOM'A HABER VER (ölmeden önce!)
        if (myRoom != null) myRoom.OnEnemyDied();

        Destroy(gameObject);
    }
}