using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : NetworkedBehaviour
{
    public UnityAction OnFire = delegate { };

    [SerializeField] Vector2[] SprayPattern;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] float TimeBetweenShots = 0.1f;
    [SerializeField] float AimResetTime = 0.2f;


    public int CurrentAmmo { get; private set; } = 30;

    //Vars
    private float TBTSTimer = 0.0f;
    private float AimResetTimer = 0.0f;

    private int BulletsInBurst = 0;

    [Header("Ammo")]
    [SerializeField] int ClipSize = 30;
    [Space]
    //Flags
    public bool bIsFiring = false;

    public Vector2 GetRecoil() { return SprayPattern[Mathf.Min(BulletsInBurst, SprayPattern.Length) - 1]; }
    public float GetRecoilTime() { return TimeBetweenShots; }

    public void OnFireInputChanged()
    {
        bIsFiring = !bIsFiring;

    }
    private void Fire()
    {
        CurrentAmmo--;
        BulletsInBurst++;
        AimResetTimer = 0.0f;

        OnFire.Invoke();

        muzzleFlash.Emit(1);
        CameraShake.instance.ViewportCameraShake(1.0f, 0.1f);
    }


    private void Update()
    {
        TBTSTimer += Time.deltaTime;

        if(TBTSTimer >= TimeBetweenShots)
        {
            TBTSTimer = 0.0f;

            if(bIsFiring)
                Fire();
        }

        AimResetTimer += Time.deltaTime;

        if(AimResetTimer >= AimResetTime)
        {
            BulletsInBurst = 0;
        }

    }

}
