using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera vc;
    public static CameraShake instance = null;

    private float shakeTimer;

    void Awake()
    {
        vc = GetComponent<CinemachineVirtualCamera>();

        if (instance != null)
            throw new Exception("Trying to create multiple instances of singleton CameraShake");
        else
            instance = this;

    }

    public void ViewportCameraShake(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }

    void Update()
    {
        if (shakeTimer <= 0)
            return;

        shakeTimer -= Time.deltaTime;

        if (shakeTimer <= 0)
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        }

    }
}
