using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BossRoomManager : MonoBehaviour
{
    [Header("Oda Alanı")]
    public Vector2 roomSize = new Vector2(20, 15);
    public Color roomGizmoColor = new Color(1, 0, 0, 0.3f);

    [Header("Kapılar")]
    public List<BossDoorInfo> doors = new List<BossDoorInfo>();

    [Header("Boss Spawn")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public float bossSpawnDelay = 1.5f;

    [Header("Player Doğma")]
    public Transform playerSpawnPoint;

    [Header("Zaman Ayarları")]
    public float doorCloseDelay = 1f;

    [Header("Sahne")]
    public string mainMenuScene = "MainMenu";

    private bool playerInside = false;
    private bool battleActive = false;
    private bool roomCleared = false;
    private BossHealth spawnedBoss;

    void Start()
    {
        foreach (var d in doors) OpenDoor(d);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (playerInside || battleActive || roomCleared) return;

        playerInside = true;
        StartCoroutine(StartBossBattle());
    }

    System.Collections.IEnumerator StartBossBattle()
    {
        Debug.Log("⏳ Boss odası! " + doorCloseDelay + " saniye sonra kapılar kapanacak...");
        yield return new WaitForSeconds(doorCloseDelay);

        if (roomCleared) yield break;

        Debug.Log("🚪 KAPILAR KAPANDI! Boss geliyor...");
        battleActive = true;

        foreach (var d in doors) CloseDoor(d);

        // SADECE BOSS SPAWN ET (düşman yok!)
        yield return new WaitForSeconds(bossSpawnDelay);
        SpawnBoss();

        // Boss ölümünü kontrol et
        StartCoroutine(CheckBossRoutine());
    }

    void SpawnBoss()
    {
        if (bossPrefab == null || bossSpawnPoint == null) return;

        GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        spawnedBoss = boss.GetComponent<BossHealth>();

        Debug.Log("💀 BOSS SPAWN EDİLDİ!");
    }

    System.Collections.IEnumerator CheckBossRoutine()
    {
        while (battleActive && !roomCleared)
        {
            yield return new WaitForSeconds(0.5f);

            if (spawnedBoss == null)
            {
                BossDefeated();
            }
        }
    }

    void BossDefeated()
    {
        if (roomCleared) return;
        roomCleared = true;
        battleActive = false;

        Debug.Log("🏆 BOSS KESİLDİ! Ana menüye dönülüyor...");

        foreach (var d in doors) OpenDoor(d);

        StartCoroutine(ReturnToMainMenu());
    }

    System.Collections.IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(mainMenuScene);
    }

    public Vector3 GetSpawnPosition()
    {
        if (playerSpawnPoint != null) return playerSpawnPoint.position;
        return transform.position;
    }

    public bool IsPlayerInside()
    {
        return playerInside;
    }

    void CloseDoor(BossDoorInfo d)
    {
        if (d.animator != null) d.animator.SetTrigger(d.closeTrigger);
        if (d.col != null) d.col.enabled = true;
    }

    void OpenDoor(BossDoorInfo d)
    {
        if (d.animator != null) d.animator.SetTrigger(d.openTrigger);
        if (d.col != null) d.col.enabled = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = roomGizmoColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(roomSize.x, roomSize.y, 0));

        if (bossSpawnPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(bossSpawnPoint.position, 0.5f);
            Gizmos.DrawLine(transform.position, bossSpawnPoint.position);
        }

        if (playerSpawnPoint != null)
        {
            Gizmos.color = Color.green;
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
public class BossDoorInfo
{
    public string name = "Kapı";
    public GameObject doorObj;
    public Animator animator;
    public string openTrigger = "Open";
    public string closeTrigger = "Close";
    public Collider2D col;
}