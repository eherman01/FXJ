using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    private static Server Instance;

    private bool Singleton()
    {
        if (!Instance)
        {
            Instance = this;
            return true;
        }
        else
        {
            Destroy(Instance);
            return false;
        }
    }

    [System.Serializable]
    public struct FXJServerParams
    {
        [SerializeField]
        int Latency;

        [SerializeField]
        int LatencyVariance;

        [Range(0,1)]
        [SerializeField]
        float PacketLossPercentage;

    }

    [SerializeField]
    private FXJServerParams ServerParams;

    void Start()
    {
        if (!Singleton())
            return;
    }

    public static void Invoke_Server_Func(MonoBehaviour Target, string FunctionName)
    {

    }

    private void RecieveServerRPC()
    {

    }

    public class FXJFunctionCall
    {
        string FunctionName;

        void InvokeFunc()
        {

        }

    }

}
