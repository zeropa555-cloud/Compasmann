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

    [Header("🆕 KARAKTER DOĞMA NOKTASI")]
    public Transform playerSpawnPoint;  // 🆕 Boş obje, karakter burada doğar
    public Color spawnGizmoColor = Color.green;

    [Header("Zaman Ayarları")]
    public float doorCloseDelay = 1f;
    public float checkInterval = 0.5f;

    private bool playerInside = false;
    private bool battleActive = false;
    private bool roomCleared = false;
    private int totalSpawned = 0;

    void Start()
    {
        foreach (var d in doors)
            OpenDoor(d);
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

        foreach (var d in doors)
            CloseDoor(d);

        StartSpawns();
        StartCoroutine(CheckEnemiesRoutine());
    }

    void StartSpawns()
    {
        totalSpawned = 0;
        foreach (var sp in spawnPoints)
        {
            if (sp != null)
            {
                sp.enabled = true;
                totalSpawned += sp.maxSpawnCount;
            }
        }
        Debug.Log("👾 Toplam " + totalSpawned + " düşman spawn edilecek");
    }

    System.Collections.IEnumerator CheckEnemiesRoutine()
    {
        while (battleActive && !roomCleared)
        {
            yield return new WaitForSeconds(checkInterval);

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            int aliveInRoom = 0;
            foreach (var e in enemies)
            {
                if (e == null) continue;
                float dist = Vector2.Distance(e.transform.position, transform.position);
                if (dist < roomSize.magnitude * 0.7f)
                    aliveInRoom++;
            }

            if (aliveInRoom <= 0 && totalSpawned > 0)
            {
                RoomCleared();
            }
        }
    }

    void RoomCleared()
    {
        roomCleared = true;
        battleActive = false;
        Debug.Log("✅ TÜM DÜŞMANLAR ÖLDÜ! Kapılar açılıyor...");

        foreach (var d in doors)
            OpenDoor(d);
    }

    void CloseDoor(DoorInfo d)
    {
        if (d.animator != null)
            d.animator.SetTrigger(d.closeTrigger);

        if (d.col != null)
            d.col.enabled = true;
    }

    void OpenDoor(DoorInfo d)
    {
        if (d.animator != null)
            d.animator.SetTrigger(d.openTrigger);

        if (d.col != null)
            d.col.enabled = false;
    }

    // 🆕 DOĞMA NOKTASINI AL (PlayerHealth çağıracak)
    public Vector3 GetSpawnPosition()
    {
        if (playerSpawnPoint != null)
            return playerSpawnPoint.position;

        return transform.position; // Fallback: Room merkezi
    }

    void OnDrawGizmos()
    {
        Gizmos.color = roomGizmoColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(roomSize.x, roomSize.y, 0));

        Gizmos.color = Color.red;
        Vector3 wallSize = new Vector3(roomSize.x - wallOffset.x * 2, roomSize.y - wallOffset.y * 2, 0);
        Gizmos.DrawWireCube(transform.position, wallSize);

        // 🆕 DOĞMA NOKTASI GÖRSEL (yeşil ok + yazı)
        if (playerSpawnPoint != null)
        {
            Gizmos.color = spawnGizmoColor;
            Gizmos.DrawWireSphere(playerSpawnPoint.position, 0.3f);
            Gizmos.DrawLine(transform.position, playerSpawnPoint.position);
            Gizmos.DrawRay(playerSpawnPoint.position, Vector3.up * 0.5f);
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