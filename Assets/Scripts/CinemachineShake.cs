using UnityEngine;
using Unity.Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance;

    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;
    private float currentAmplitude;

    void Awake()
    {
        Instance = this;
        noise = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        if (noise == null) return;

        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            noise.AmplitudeGain = currentAmplitude;
            noise.FrequencyGain = 10f;
        }
        else
        {
            noise.AmplitudeGain = 0f;
            noise.FrequencyGain = 0f;
        }
    }

    public void Shake(float duration, float amplitude)
    {
        if (noise == null) return;

        shakeTimer = duration;
        currentAmplitude = amplitude;
    }
}