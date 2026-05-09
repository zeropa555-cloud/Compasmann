using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Ayarları")]
    public GameObject enemyPrefab;
    public int maxSpawnCount = 5;        // 🆕 maxCount yerine maxSpawnCount
    public float spawnInterval = 2f;
    public float startDelay = 1f;

    [Header("Görsel")]
    public Color gizmoColor = Color.yellow;
    public float gizmoSize = 0.3f;

    private int spawnedCount = 0;
    private float nextSpawnTime;
    private bool isActive = false;

    void OnEnable()
    {
        isActive = true;
        nextSpawnTime = Time.time + startDelay;
    }

    void Update()
    {
        if (!isActive) return;
        if (enemyPrefab == null) return;
        if (spawnedCount >= maxSpawnCount) return;  // 🆕 maxSpawnCount kullan

        if (Time.time >= nextSpawnTime)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            spawnedCount++;
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
        Gizmos.DrawRay(transform.position, Vector3.up * gizmoSize * 2);
    }
}