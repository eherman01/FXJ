using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehaviour : NetworkedBehaviour
{
    [SerializeField] Material debugServerMat = null;
    [SerializeField] int maxHealth = 3;
    [SerializeField] Vector2 clamps = new Vector2(-9.0f, 9.0f);
    [SerializeField] float moveSpeed;
    [SerializeField] Animator animator;
    [SerializeField] bool bMoveWhenDead = true;

    private int health = 0;
    private int movementDir = 1;
    private Vector3 cachedPosition;
    private int lastPositionUpdateFrame;

    private void Start()
    {
        base.Init();

        if(GetNetMode() == ENetMode.SERVER)
            foreach (MeshRenderer r in transform.GetComponentsInChildren<MeshRenderer>())
            {
                if (Server.IsDebugMode())
                    r.material = debugServerMat;
                else
                    r.enabled = false;
            }

    }

    private void Update()
    {
        UpdateBase();

        if (GetNetMode() != ENetMode.SERVER)
            return;

        if (!TargetManager.instance.bIsGameActive)
            return;

        if (!IsAlive() && !bMoveWhenDead)
            return;

        float moveDelta = movementDir * moveSpeed * Time.deltaTime;

        if (movementDir == -1 && transform.position.z - moveDelta <= clamps.x || movementDir == 1 && transform.position.z + moveDelta >= clamps.y)
        {
            movementDir *= -1;
            moveDelta *= -1;
        }

        transform.position += new Vector3(0, 0, moveDelta);
    }

    public void OnHit(Vector3 hitPos)
    {
        if (!IsAlive())
            return;

        bool bHeadShot = false;
        if (hitPos.y > transform.position.y + 0.37f)
            bHeadShot = true;

        if (bHeadShot)
            health -= 3;
        else
            health -= 1;

        if (health <= 0)
            Kill();
    }

    public void Kill()
    {
        if (GetNetMode() == ENetMode.SERVER)
        {
            Server.Invoke_Client_Func_Reliable(this, "Kill_RPC_Client", null);
            TargetManager.instance.OnTargetKilled();
        }

        animator.SetBool("isAlive", false);

    }

    public void Deploy()
    {
        if(GetNetMode() == ENetMode.SERVER)
            Server.Invoke_Client_Func_Reliable(this, "Deploy_RPC_Client", null);

        animator.SetBool("isAlive", true);
        health = maxHealth;
    }

    private bool IsAlive() { return health > 0; }

    protected override void NetworkTick()
    {
        if (GetNetMode() == ENetMode.SERVER)
        {
            Server.Invoke_Client_Func_Unreliable(this, "SetPosition_RPC_Client", new object[] { transform.position, Time.frameCount });
        }

    }
    private void Deploy_RPC_Client()
    {
        Deploy();
    }

    private void Kill_RPC_Client()
    {
        Kill();
    }

    private void SetPosition_RPC_Client(Vector3 position, int frameCount)
    {
        //Only use latest movement command if we recieve two on the same frame
        if (frameCount > lastPositionUpdateFrame)
        {
            cachedPosition = position;
            lastPositionUpdateFrame = frameCount;
            SetPosition_Internal(cachedPosition);

        }
        else
        {
            Debug.Log("out of order movement command dropped");
        }
    }

    private void SetPosition_Internal(Vector3 position)
    {
        transform.position = position;

    }


}
