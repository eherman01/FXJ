using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera vc;
    public static CameraShake instance = null;
    private float shakeTimer;

    private float cachedIntensity = 1.0f;
    private float oldFOV;
    private float timeMultiplier;

    [SerializeField] AnimationCurve cameraShakeCurve = new AnimationCurve();

    void Awake()
    {
        vc = GetComponent<CinemachineVirtualCamera>();

        oldFOV = vc.m_Lens.FieldOfView;

        if (instance != null)
            throw new Exception("Trying to create multiple instances of singleton CameraShake");    //cursed way of making a singleton but i cba
        else
            instance = this;

    }

    public void ViewportCameraShake(float intensity, float time)
    {
        cachedIntensity = intensity;
        shakeTimer = time;
        timeMultiplier = cameraShakeCurve.keys[cameraShakeCurve.length - 1].time / time;
    }

    void Update()
    {
        if (shakeTimer <= 0)
            return;

        shakeTimer -= Time.deltaTime;

        vc.m_Lens.FieldOfView = oldFOV + (cameraShakeCurve.Evaluate(shakeTimer * timeMultiplier) * cachedIntensity);

        if (shakeTimer <= 0)
        {
            vc.m_Lens.FieldOfView = oldFOV;
            shakeTimer = 0;
        }

    }
}
