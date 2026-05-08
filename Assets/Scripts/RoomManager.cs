using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Oda Ayarları")]
    public GameObject[] doors; // Inspector'dan kapı objelerini buraya sürükle
    public int totalWaves = 3; // Toplam kaç kez spawn olacak? (Örn: 3 dalga)
    
    private int currentWave = 0; // Kaçıncı dalgadayız?
    private int activeEnemies = 0; // Sahnede şu an kaç canlı düşman var?
    private bool isRoomCleared = false; // Oda tamamen bitti mi?

    void Start()
    {
        CloseDoors(); // Başlangıçta oyuncu içeri girince kapıları kapat
        StartNextWave(); // 1. Dalgayı başlat
    }

    public void StartNextWave()
    {
        if (currentWave < totalWaves)
        {
            currentWave++;
            Debug.Log("Dalga Başladı: " + currentWave);
            SpawnEnemies(); // Düşmanları yarat
        }
        else
        {
            // Dalga sayısı bittiyse ve son düşmanlar da öldüyse:
            OpenDoors();
            isRoomCleared = true;
            Debug.Log("Oda Temizlendi, Kapılar Açıldı!");
        }
    }

    void SpawnEnemies()
    {
        // NOT: Kendi düşman yaratma (Instantiate) kodunu buraya ekleyeceksin.
        // Şimdilik sistemin çalışması için test mantığı kuruyoruz:
        
        int enemiesToSpawnThisWave = 3; // Her dalgada kaç düşman çıkacak?

        for (int i = 0; i < enemiesToSpawnThisWave; i++)
        {
            // Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            
            activeEnemies++; // Doğan her düşman için sayacı 1 artırıyoruz!
        }
    }

    // Herhangi bir düşman öldüğünde o düşman bu kodu tetikleyecek
    public void EnemyDied()
    {
        activeEnemies--; // Sahnede bir düşman azaldı

        // Eğer sahnede düşman kalmadıysa ve odadaki tüm dalgalar bitmediyse:
        if (activeEnemies <= 0 && !isRoomCleared)
        {
            StartNextWave(); // Sıradaki düşman dalgasını getir!
        }
    }

    void OpenDoors()
    {
        foreach (GameObject door in doors)
        {
            door.SetActive(false); // Kapıları deaktif et (Geçiş yolu açılır)
        }
    }

    void CloseDoors()
    {
        foreach (GameObject door in doors)
        {
            door.SetActive(true); // Kapıları aktif et (Geçiş kapanır)
        }
    }
}