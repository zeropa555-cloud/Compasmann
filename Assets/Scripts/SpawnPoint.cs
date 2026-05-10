using UnityEngine;
using System.Collections.Generic;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Ayarları")]
    public List<GameObject> enemyPrefabs = new List<GameObject>(); // 🆕 2+ düşman prefabı
    public int maxSpawnCount = 5;
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
        if (enemyPrefabs.Count == 0) return; // 🆕 Liste boşsa dur
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
        // 🆕 RASTGELE DÜŞMAN SEÇ (0 veya 1)
        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        GameObject selectedPrefab = enemyPrefabs[randomIndex];

        if (selectedPrefab != null)
            Instantiate(selectedPrefab, transform.position, Quaternion.identity);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
        Gizmos.DrawRay(transform.position, Vector3.up * gizmoSize * 2);
    }
}