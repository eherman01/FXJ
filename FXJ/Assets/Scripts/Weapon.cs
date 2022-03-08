using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : NetworkedBehaviour
{

    [SerializeField] ParticleSystem muzzleFlash;

    [SerializeField] float TimeBetweenShots = 0.1f;

    private float TBTSTimer = 0.0f;

    bool bIsFiring = false;

    public void OnFireInputChanged()
    {
        bIsFiring = !bIsFiring;

    }
    private void Fire()
    {
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
    }

}
