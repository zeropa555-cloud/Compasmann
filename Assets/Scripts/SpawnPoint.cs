using UnityEngine;
using System.Collections.Generic;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Ayarları")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public int maxSpawnCount = 5;
    public float spawnInterval = 2f;
    public float startDelay = 1f;

    [Header("Görsel")]
    public Color gizmoColor = Color.yellow;
    public float gizmoSize = 0.3f;

    private int spawnedCount = 0;
    private float nextSpawnTime;
    private bool isActive = false;
    private RoomManager myRoom;

    void Awake()
    {
        // 🆕 BAŞLANGIÇTA KAPALI! RoomManager açana kadar spawn etmez
        enabled = false;
    }

    public void SetRoom(RoomManager room)
    {
        myRoom = room;
    }

    void OnEnable()
    {
        isActive = true;
        nextSpawnTime = Time.time + startDelay;
    }

    void Update()
    {
        if (!isActive) return;
        if (enemyPrefabs.Count == 0) return;
        if (spawnedCount >= maxSpawnCount) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            spawnedCount++;
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        GameObject prefab = enemyPrefabs[randomIndex];
        if (prefab == null) return;

        GameObject enemy = Instantiate(prefab, transform.position, Quaternion.identity);

        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        if (health != null) health.SetRoom(myRoom);

        if (myRoom != null) myRoom.OnEnemySpawned();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
        Gizmos.DrawRay(transform.position, Vector3.up * gizmoSize * 2);
    }
}