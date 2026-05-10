using UnityEngine;

public class ThemeMusicManager : MonoBehaviour
{
    private static ThemeMusicManager instance;
    public AudioSource musicSource;

    void Awake()
    {
        // Singleton yapısı: Eğer sahnede zaten bir müzik yöneticisi varsa yenisini yok et.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Sahneler arası geçişte objeyi koru.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play(); // Müziği başlat.
        }
    }
}