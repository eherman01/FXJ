using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedTransform : NetworkedBehaviour
{
    [SerializeField]
    private bool ReplicateMovement = true;

    [SerializeField]
    private ENetMode ControlledBy = ENetMode.SERVER;

    private Vector3 cachedPosition;
    private int lastPositionUpdateFrame;

    void Start()
    {
        base.Init();
    }

    private void Update()
    {
        UpdateBase();
    }

    protected override void NetworkTick()
    {

        if(GetNetMode() == ENetMode.CLIENT && ReplicateMovement)
        {
            Server.Invoke_Server_Func_UDP(this, "SetPosition_RPC_Server", new object[] { transform.position, Time.frameCount });
        }

        if (GetNetMode() == ENetMode.SERVER && ReplicateMovement)
        {
            Server.Invoke_Client_Func_UDP(this, "SetPosition_RPC_Client", new object[] { transform.position, Time.frameCount });
        }

    }

    private void SetPosition_RPC_Server(Vector3 position, int frameCount)
    {
        //Only use latest movement command if we recieve two on the same frame
        if(frameCount > lastPositionUpdateFrame)
        {
            //here is where we would check for illegal movements and correct the player
            //since cheating prevention is not part of the scope of this study, we assume that the client never makes an illegal move
            cachedPosition = position;
            lastPositionUpdateFrame = frameCount;

            //This is also where we would apply server side lag compensation if it was part of the scope of the study
            if (GetNetMode() != ControlledBy)
            {
                SetPosition_Internal(cachedPosition);
            }
        }
        else
        {
            Debug.Log("out of order movement command dropped");
        }

    }

    private void SetPosition_RPC_Client(Vector3 position, int frameCount)
    {
        //Only use latest movement command if we recieve two on the same frame
        if (frameCount > lastPositionUpdateFrame)
        {
            cachedPosition = position;
            lastPositionUpdateFrame = frameCount;

            if(GetNetMode() != ControlledBy)
            {
                SetPosition_Internal(cachedPosition);
            }

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
