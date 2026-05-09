using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        // 1. Hedef FPS = 60 (standart, tüm PC'lerde ayný)
        Application.targetFrameRate = 60;

        // 2. VSync AÇIK (monitör yenileme hýzýna kilitler, tearing olmaz)
        // 0 = kapalý, 1 = her frame'de sync, 2 = her 2. frame
        QualitySettings.vSyncCount = 1;
    }
}