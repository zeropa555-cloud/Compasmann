using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    [Header("Kapı")]
    public GameObject doorObject;           // Kapı objesi
    public Animator doorAnimator;           // 🆕 Kapının Animator'u
    public string openTrigger = "Open";     // 🆕 Açılma trigger adı
    public string closeTrigger = "Close";   // 🆕 Kapanma trigger adı
    public Collider2D doorCollider;         // Kapı collider (geçişi engelleyen)

    [Header("Spawn Noktaları")]
    public List<SpawnPoint> spawnPoints;

    [Header("Oda Ayarları")]
    public bool autoStartOnEnter = true;
    public bool roomCleared = false;

    private bool playerInside = false;
    private bool waveStarted = false;

    void Start()
    {
        // Başlangıçta kapı açık (animasyonlu)
        OpenDoor();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerInside && !roomCleared)
        {
            playerInside = true;
            if (autoStartOnEnter && !waveStarted)
                StartRoom();
        }
    }

    public void StartRoom()
    {
        if (waveStarted) return;
        waveStarted = true;

        Debug.Log("🚪 ODA KAPANIYOR!");

        CloseDoor();
        StartSpawns();
        StartCoroutine(CheckEnemiesRoutine());
    }

    void StartSpawns()
    {
        foreach (var sp in spawnPoints)
        {
            if (sp != null) sp.enabled = true; // Spawn'ları aktif et
        }
    }

    System.Collections.IEnumerator CheckEnemiesRoutine()
    {
        while (!roomCleared)
        {
            yield return new WaitForSeconds(1f);

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (enemies.Length <= 0 && waveStarted)
            {
                RoomCleared();
            }
        }
    }

    void RoomCleared()
    {
        roomCleared = true;
        Debug.Log("✅ ODA TEMİZLENDİ! Kapı açılıyor...");
        OpenDoor();
    }

    void CloseDoor()
    {
        // 🎬 KAPANMA ANİMASYONU
        if (doorAnimator != null)
            doorAnimator.SetTrigger(closeTrigger);
        else
            Debug.LogWarning("Kapı Animator yok!");

        // Collider'ı hemen aktif et (geçişi engelle)
        if (doorCollider != null)
            doorCollider.enabled = true;
    }

    void OpenDoor()
    {
        // 🎬 AÇILMA ANİMASYONU
        if (doorAnimator != null)
            doorAnimator.SetTrigger(openTrigger);
        else
            Debug.LogWarning("Kapı Animator yok!");

        // Collider'ı kapat (geçiş serbest)
        if (doorCollider != null)
            doorCollider.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(10, 10, 0));
    }
}