using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Ayarlarý")]
    public GameObject enemyPrefab;      // Düþman prefabý
    public float spawnDelay = 1f;       // Ýlk spawn gecikmesi
    public float spawnInterval = 3f;    // Spawn aralýðý
    public int maxSpawnCount = 5;       // Max kaç düþman çýksýn

    [Header("Görsel")]
    public Color gizmoColor = Color.yellow;
    public float gizmoSize = 0.3f;

    private int spawnedCount = 0;
    private float nextSpawnTime;

    void Start()
    {
        nextSpawnTime = Time.time + spawnDelay;
    }

    void Update()
    {
        if (enemyPrefab == null) return;
        if (spawnedCount >= maxSpawnCount) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        spawnedCount++;

        // Düþman ölünce sayý azalsýn diye event baðla (opsiyonel)
        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
        {
            // Basit yöntem: RoomManager total sayýyý takip eder
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
        Gizmos.DrawRay(transform.position, Vector3.up * gizmoSize * 2);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, "SPAWN");
#endif
    }
}