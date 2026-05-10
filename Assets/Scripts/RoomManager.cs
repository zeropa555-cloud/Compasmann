using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    [Header("Oda Alanı")]
    public Vector2 roomSize = new Vector2(12, 8);
    public Color roomGizmoColor = new Color(0, 1, 1, 0.3f);

    [Header("Duvar Sınırları")]
    public Vector2 wallOffset = new Vector2(0.5f, 0.5f);

    [Header("Kapılar")]
    public List<DoorInfo> doors = new List<DoorInfo>();

    [Header("Spawn Noktaları")]
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    [Header("Karakter Doğma Noktası")]
    public Transform playerSpawnPoint;
    public Color spawnGizmoColor = Color.green;

    [Header("Zaman Ayarları")]
    public float doorCloseDelay = 1f;

    // 🆕 KESİN SAYAÇLAR
    private int enemiesSpawned = 0;   // Toplam kaç düşman çıktı
    private int enemiesKilled = 0;    // Kaç düşman öldü
    private bool playerInside = false;
    private bool battleActive = false;
    private bool roomCleared = false;

    void Start()
    {
        foreach (var d in doors) OpenDoor(d);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (playerInside || battleActive || roomCleared) return;

        playerInside = true;
        StartCoroutine(StartBattleDelayed());
    }

    System.Collections.IEnumerator StartBattleDelayed()
    {
        Debug.Log("⏳ " + doorCloseDelay + " saniye sonra kapılar kapanacak...");
        yield return new WaitForSeconds(doorCloseDelay);
        if (roomCleared) yield break;

        Debug.Log("🚪 KAPILAR KAPANDI!");
        battleActive = true;
        foreach (var d in doors) CloseDoor(d);

        StartSpawns();
    }

    void StartSpawns()
    {
        enemiesSpawned = 0;
        enemiesKilled = 0;

        foreach (var sp in spawnPoints)
        {
            if (sp != null)
            {
                sp.enabled = true;
                // SpawnPoint kendi sayısını RoomManager'a bildirecek
                sp.SetRoom(this);
            }
        }
    }

    // 🆕 DÜŞMAN DOĞUNCA ÇAĞRILIR
    public void OnEnemySpawned()
    {
        enemiesSpawned++;
        Debug.Log("👾 Düşman doğdu! Toplam: " + enemiesSpawned);
    }

    // 🆕 DÜŞMAN ÖLÜNCE ÇAĞRILIR
    public void OnEnemyDied()
    {
        enemiesKilled++;
        Debug.Log("💀 Düşman öldü! Ölen: " + enemiesKilled + " / " + enemiesSpawned);

        // 🎯 HEPSİ ÖLÜNCE AÇ (enemiesSpawned > 0 garantisi)
        if (enemiesKilled >= enemiesSpawned && enemiesSpawned > 0)
        {
            RoomCleared();
        }
    }

    void RoomCleared()
    {
        if (roomCleared) return;
        roomCleared = true;
        battleActive = false;
        Debug.Log("✅ ODA TEMİZLENDİ! Kapılar açılıyor...");

        foreach (var d in doors) OpenDoor(d);
    }

    void CloseDoor(DoorInfo d)
    {
        if (d.animator != null) d.animator.SetTrigger(d.closeTrigger);
        if (d.col != null) d.col.enabled = true;
    }

    void OpenDoor(DoorInfo d)
    {
        if (d.animator != null) d.animator.SetTrigger(d.openTrigger);
        if (d.col != null) d.col.enabled = false;
    }

    public Vector3 GetSpawnPosition()
    {
        if (playerSpawnPoint != null) return playerSpawnPoint.position;
        return transform.position;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = roomGizmoColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(roomSize.x, roomSize.y, 0));

        Gizmos.color = Color.red;
        Vector3 wallSize = new Vector3(roomSize.x - wallOffset.x * 2, roomSize.y - wallOffset.y * 2, 0);
        Gizmos.DrawWireCube(transform.position, wallSize);

        if (playerSpawnPoint != null)
        {
            Gizmos.color = spawnGizmoColor;
            Gizmos.DrawWireSphere(playerSpawnPoint.position, 0.3f);
            Gizmos.DrawLine(transform.position, playerSpawnPoint.position);
        }

        foreach (var d in doors)
        {
            if (d.doorObj != null)
            {
                Gizmos.color = roomCleared ? Color.green : Color.yellow;
                Gizmos.DrawLine(transform.position, d.doorObj.transform.position);
            }
        }
    }
}

[System.Serializable]
public class DoorInfo
{
    public string name = "Kapı";
    public GameObject doorObj;
    public Animator animator;
    public string openTrigger = "Open";
    public string closeTrigger = "Close";
    public Collider2D col;
}