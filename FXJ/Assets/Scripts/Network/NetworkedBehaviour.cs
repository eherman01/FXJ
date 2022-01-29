using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedBehaviour : MonoBehaviour
{
    private NetworkedBehaviour ServerInstance; 
    private NetworkedBehaviour ClientInstance;

    public bool IsNetworked = true;

    public enum ENetMode
    {
        NONE = 0,
        CLIENT = 1,
        SERVER = 2,
        MAX
    }

    private ENetMode NetMode = ENetMode.NONE;

    public ENetMode GetNetMode() { return NetMode; }
    public NetworkedBehaviour GetServerInstance() { return ServerInstance; }
    public NetworkedBehaviour GetClientInstance() { return ClientInstance; }

    protected virtual void NetworkTick() { }
    private float TickRate_Internal;
    private float TimeSinceLastTick;

    protected virtual void Init()
    {
        TimeSinceLastTick = 0;

        if (GetNetMode() == ENetMode.CLIENT)
            return;

        if (!IsNetworked)
        {
            NetMode = ENetMode.NONE;
            return;
        }


        NetMode = ENetMode.SERVER;
        ServerInstance = this;

        GameObject ClientObj = GetClientObj();
        ClientInstance = ClientObj.GetComponent(GetType()) as NetworkedBehaviour;
        ClientInstance.ServerInstance = this;
        ClientInstance.NetMode = ENetMode.CLIENT;

    }
    
    protected void UpdateBase()
    {

        TimeSinceLastTick += Time.deltaTime;
        if(TimeSinceLastTick >= 1.0f / Server.GetServerParams().TickRate)
        {
            NetworkTick();
            TimeSinceLastTick = 0;
        }

    }

    private GameObject GetClientObj()
    {
        GameObject ClientObj = null;

        foreach (NetworkedBehaviour nb in GetComponents<NetworkedBehaviour>())
        {
            if (nb.ClientInstance)
            {
                ClientObj = nb.ClientInstance.gameObject;
                break;
            }

        }

        if (ClientObj == null)
        {
            ClientObj = Instantiate(gameObject);
            ClientObj.name = "Client_" + gameObject.name;
        }

        return ClientObj;
    }

}
