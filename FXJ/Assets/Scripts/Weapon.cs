using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : NetworkedBehaviour
{
    public UnityAction OnFire = delegate { };

    [SerializeField] Vector2[] SprayPattern;
    [SerializeField] float SprayPatternScale = 1.0f;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] float TimeBetweenShots = 0.1f;
    [SerializeField] float AimResetTime = 0.2f;

    public int CurrentAmmo { get; private set; } = 30;

    //Vars
    private float TBTSTimer = 0.0f;
    private float AimResetTimer = 0.0f;

    private int BulletsInBurst = 0;

    //bulletholes
    private static Queue<GameObject> bulletHolesInScene = new Queue<GameObject>();

    [Header("Weapon")]
    [SerializeField] GameObject bulletHole = null;
    [SerializeField] GameObject serverBulletHole = null;
    [Space]
    [Header("Ammo")]
    [SerializeField] int ClipSize = 30;
    [Space]
    //Flags
    public bool bIsFiring = false;

    private void Start()
    {
        base.Init();
    }

    public Vector2 GetRecoil() { return SprayPattern[Mathf.Min(BulletsInBurst, SprayPattern.Length) - 1] * SprayPatternScale; }
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

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("Server");
        if (Physics.Raycast(ray, out hit, 10000.0f, layerMask))
        {
            GameObject bhole = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
            bhole.transform.parent = hit.transform;
            bulletHolesInScene.Enqueue(bhole);

            if (bulletHolesInScene.Count > 100)
                Destroy(bulletHolesInScene.Dequeue());

        }

        Server.Invoke_Server_Func_Reliable(this, "Fire_RPC_Server", new object[] { ray });

        OnFire.Invoke();
        muzzleFlash.Emit(1);
        CameraShake.instance.ViewportCameraShake(1.0f, 0.1f);
    }
    private void Fire_RPC_Server(Ray ray)
    {
        RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("Client");
        if (Physics.Raycast(ray, out hit, 10000.0f, layerMask))
        {
            if (Server.IsDebugMode())
            {
                GameObject bhole = Instantiate(serverBulletHole, hit.point, Quaternion.LookRotation(hit.normal));
                bhole.transform.parent = hit.transform;
                bulletHolesInScene.Enqueue(bhole);

                if (bulletHolesInScene.Count > 100)
                    Destroy(bulletHolesInScene.Dequeue());
            }

            TargetBehaviour target = hit.transform.GetComponentInParent<TargetBehaviour>();
            if (target)
                target.OnHit(hit.point);

        }
    }


    private void Update()
    {
        if (GetNetMode() == ENetMode.SERVER)
            return;

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
