using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Sahne İsimleri")]
    public string level1Scene = "Level1";

    [Header("Butonlar")]
    public GameObject playButton;
    public GameObject quitButton;

    void Start()
    {
        // Zamanı normal yap (eğer pause menüden gelindiysye)
        Time.timeScale = 1f;
    }

    public void PlayGame()
    {
        Debug.Log("🎮 Oyun başlıyor...");
        SceneManager.LoadScene(level1Scene);
    }

    public void QuitGame()
    {
        Debug.Log("👋 Çıkış yapılıyor...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}