using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetTestObj : NetworkedBehaviour
{

    [SerializeField] private Renderer CubeRenderer;
    [SerializeField] private Material ClientMaterial;
    [SerializeField] private Material ServerMaterial;


    private void Start()
    {
        base.Init();

        if(GetNetMode() == ENetMode.CLIENT)
            Server.Invoke_Server_Func_UDP(this, "PrintString_Server", new object[] { "Inshallah" });
    }

    private void Update()
    {
        if(GetNetMode() == ENetMode.CLIENT)
        {

        }

    }

    private void PrintString_Server(string msg)
    {
        CubeRenderer.material = ServerMaterial;
        Server.Invoke_Client_Func_UDP(this, "PrintString_Client", new object[] { "Inshallah" });

    }

    private void PrintString_Client(string msg)
    {
        CubeRenderer.material = ClientMaterial;

    }


}
