
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private CinemachineBrain brain;
    private Transform camTransform;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Main Camera'yi bul
        camTransform = Camera.main.transform;
        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(DoShake(duration, magnitude));
    }

    IEnumerator DoShake(float duration, float magnitude)
    {
        // 🎬 CINEMACHINE'I DURDUR (yoksa sallama gözükmez!)
        if (brain != null) brain.enabled = false;

        Vector3 originalPos = camTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            camTransform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 🔄 Kamerayı eski yerine koy
        camTransform.localPosition = originalPos;

        // 🎬 CINEMACHINE'I TEKRAR AÇ
        if (brain != null) brain.enabled = true;
    }
}