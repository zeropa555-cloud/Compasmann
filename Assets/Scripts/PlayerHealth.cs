using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider;
    public float invincibilityDuration = 0.5f;
    public bool isInvincible { get; private set; }

    private Animator anim;

    void Awake()
    {
        currentHealth = maxHealth;

        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        if (healthSlider != null) healthSlider.maxValue = maxHealth;
    }

    void Start() { UpdateHealthUI(); }

    public void TakeDamage(float amount)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthUI();

        if (anim != null) anim.SetTrigger("Hit");

        StartCoroutine(InvincibilityFrames());
        if (currentHealth <= 0) Die();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null) healthSlider.value = currentHealth;
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("Oldun!");

        RoomManager nearestRoom = FindNearestRoom();

        if (nearestRoom != null)
        {
            transform.position = nearestRoom.GetSpawnPosition();
            Debug.Log("🔄 " + nearestRoom.name + " odasındaki doğma noktasında yeniden doğdun!");
        }
        else
        {
            transform.position = Vector3.zero;
        }

        currentHealth = maxHealth;
        UpdateHealthUI();
        isInvincible = false;
    }

    RoomManager FindNearestRoom()
    {
        RoomManager[] rooms = FindObjectsOfType<RoomManager>();
        RoomManager nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var room in rooms)
        {
            float dist = Vector2.Distance(transform.position, room.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = room;
            }
        }
        return nearest;
    }
}