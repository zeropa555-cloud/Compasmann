using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider;
    public float invincibilityDuration = 0.5f;
    public bool isInvincible { get; private set; }

    [Header("Yeniden Doğma")]
    public int extraLives = 2;           // 🆕 2 kere yeniden doğma (toplam 3 can)
    public Text livesText;               // 🆕 UI (isteğe bağlı, Canvas'e Text ekle)
    public float respawnDelay = 1f;    // 🆕 Ölünce bekleme süresi

    private int currentLives;
    private Animator anim;

    void Awake()
    {
        currentHealth = maxHealth;
        currentLives = extraLives;

        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        if (healthSlider != null) healthSlider.maxValue = maxHealth;
        UpdateLivesUI();
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

    void UpdateLivesUI()
    {
        if (livesText != null) livesText.text = "❤ " + (currentLives + 1);
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    void Die()
    {
        currentLives--;
        UpdateLivesUI();

        // 🆕 HAK BİTTİYSE OYUNU SIFIRLA (Restart)
        if (currentLives < 0)
        {
            Debug.Log("💀 TÜM CANLAR BİTTİ! Oyun sıfırdan başlıyor...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        Debug.Log("💀 Öldün! Kalan hak: " + (currentLives + 1));
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);

        currentHealth = maxHealth;
        UpdateHealthUI();

        // 🆕 BOSS ODASINDA MIYIZ?
        BossRoomManager bossRoom = FindObjectOfType<BossRoomManager>();
        if (bossRoom != null && bossRoom.IsPlayerInside())
        {
            transform.position = bossRoom.GetSpawnPosition();
            Debug.Log("🔄 Boss odasında yeniden doğdun!");
        }
        else
        {
            // Normal oda spawn'u
            RoomManager nearestRoom = FindNearestRoom();
            if (nearestRoom != null)
                transform.position = nearestRoom.GetSpawnPosition();
            else
                transform.position = Vector3.zero;
        }

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