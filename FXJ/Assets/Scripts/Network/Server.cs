using System.Reflection;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        public int TickRate;

        public int Latency;

        public int LatencyVariance;

        [Range(0,1)]
        public float PacketLossPercentage;

    }

    [SerializeField]
    private FXJServerParams ServerParams;
    public bool bDebugMode = true;
    public static FXJServerParams GetServerParams() { return Instance.ServerParams; }
    public static bool IsDebugMode() { return Instance.bDebugMode; }
    void Awake()
    {
        if (!Singleton())
            return;
    }

    public static async void Invoke_Server_Func_Unreliable(NetworkedBehaviour Target, string FunctionName, object[] args)
    {
        NetworkedBehaviour ServerTarget = Target.GetServerInstance();

        if (ServerTarget == null)
        {
            Debug.LogError("Server instance not found for object " + Target);
            return;
        } 
        else if(ServerTarget != Target) //Apply ping and packet loss if not invoked from server
        {
            //todo: apply packet loss

            //Apply ping
            FXJServerParams ServerParams = Instance.ServerParams;
            int Latency = Mathf.Max(0, UnityEngine.Random.Range(ServerParams.Latency - ServerParams.LatencyVariance, ServerParams.Latency + ServerParams.LatencyVariance));
            await Task.Delay(Latency);

        }

        //Invoke function
        MethodInfo Func = ServerTarget.GetType().GetMethod(FunctionName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if(Func == null)
        {
            Debug.LogError("Function " + FunctionName + " not found on object of type " + ServerTarget.GetType());
            return;
        }

        Func.Invoke(ServerTarget, args);

    }

    public static async void Invoke_Client_Func_Unreliable(NetworkedBehaviour Target, string FunctionName, object[] args)
    {
        NetworkedBehaviour ClientTarget = Target.GetClientInstance();

        if (ClientTarget == null)
        {
            Debug.LogError("Client instance not found for object " + Target);
            return;
        }
        else if (ClientTarget != Target) //Apply ping and packet loss if not invoked from client
        {
            //todo: apply packet loss

            //Apply ping
            FXJServerParams ServerParams = Instance.ServerParams;
            int Latency = Mathf.Max(0, UnityEngine.Random.Range(ServerParams.Latency - ServerParams.LatencyVariance, ServerParams.Latency + ServerParams.LatencyVariance));
            await Task.Delay(Latency);
        }

        //todo: apply packet loss
        //todo: apply ping

        MethodInfo Func = ClientTarget.GetType().GetMethod(FunctionName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (Func == null)
        {
            Debug.LogError("Function " + FunctionName + " not found on object of type " + ClientTarget.GetType());
            return;
        }

        Func.Invoke(ClientTarget, args);

    }

    //todo: TCP
    public static async void Invoke_Server_Func_Reliable(NetworkedBehaviour Target, string FunctionName, object[] args)
    {
        NetworkedBehaviour ServerTarget = Target.GetServerInstance();

        if (ServerTarget == null)
        {
            Debug.LogError("Server instance not found for object " + Target);
            return;
        }
        else if (ServerTarget != Target) //Apply ping and packet loss if not invoked from server
        {
            //todo: apply packet loss

            //Apply ping
            FXJServerParams ServerParams = Instance.ServerParams;
            int Latency = Mathf.Max(0, UnityEngine.Random.Range(ServerParams.Latency - ServerParams.LatencyVariance, ServerParams.Latency + ServerParams.LatencyVariance));
            await Task.Delay(Latency);

        }

        //Invoke function
        MethodInfo Func = ServerTarget.GetType().GetMethod(FunctionName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (Func == null)
        {
            Debug.LogError("Function " + FunctionName + " not found on object of type " + ServerTarget.GetType());
            return;
        }

        Func.Invoke(ServerTarget, args);

    }

    //todo: Reliable communication
    public static async void Invoke_Client_Func_Reliable(NetworkedBehaviour Target, string FunctionName, object[] args)
    {
        NetworkedBehaviour ClientTarget = Target.GetClientInstance();

        if (ClientTarget == null)
        {
            Debug.LogError("Client instance not found for object " + Target);
            return;
        }
        else if (ClientTarget != Target) //Apply ping and packet loss if not invoked from client
        {
            //todo: apply packet loss

            //Apply ping
            FXJServerParams ServerParams = Instance.ServerParams;
            int Latency = Mathf.Max(0, UnityEngine.Random.Range(ServerParams.Latency - ServerParams.LatencyVariance, ServerParams.Latency + ServerParams.LatencyVariance));
            await Task.Delay(Latency);
        }

        //todo: apply packet loss
        //todo: apply ping

        MethodInfo Func = ClientTarget.GetType().GetMethod(FunctionName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (Func == null)
        {
            Debug.LogError("Function " + FunctionName + " not found on object of type " + ClientTarget.GetType());
            return;
        }

        Func.Invoke(ClientTarget, args);

    }

}
