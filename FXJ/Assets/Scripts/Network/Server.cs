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

        [Range(0,1)]
        public float OutOfOrderChance;

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

    public List<NetworkPacket> inBuffer = new List<NetworkPacket>();
    public List<NetworkPacket> outBuffer = new List<NetworkPacket>();
    public void BufferIncomingPacket(NetworkPacket packet)
    {
        inBuffer.Add(packet);

        int i = 0;
        while(i < inBuffer.Count - 1)
        {
            NetworkPacket first = inBuffer[i];
            NetworkPacket second = inBuffer[i+1];

            if (UnityEngine.Random.Range(0.0f, 1.0f) <= ServerParams.OutOfOrderChance)
            {
                inBuffer[i] = second;
                inBuffer[i+1] = first;
                Debug.Log("swap");
            }
            else
            {
                i++;
            }

        }

    }
    public void BufferOutgoingPacket(NetworkPacket packet)
    {
        outBuffer.Add(packet);

        int i = 0;
        while (i < outBuffer.Count - 1)
        {
            NetworkPacket first = outBuffer[i];
            NetworkPacket second = outBuffer[i + 1];

            if (UnityEngine.Random.Range(0.0f, 1.0f) <= ServerParams.OutOfOrderChance)
            {
                outBuffer[i] = second;
                outBuffer[i + 1] = first;
            }
            else
            {
                i++;
            }

        }
    }

    private void Update()
    {
        ApplyLatency();
    }

    private void ApplyLatency()
    {

        //Ingoing packet latency
        while (inBuffer.Count > 0)
        {
            NetworkPacket pac = inBuffer[0];

            if (Time.timeSinceLevelLoadAsDouble > pac.timestamp + ServerParams.Latency * 0.001)
            {
                inBuffer.RemoveAt(0);
                pac.Invoke();
            }
            else
            {
                break;
            }
        }

        //Outgoing packet latency
        while (outBuffer.Count > 0)
        {
            NetworkPacket pac = outBuffer[0];
            if (Time.timeSinceLevelLoadAsDouble > pac.timestamp + ServerParams.Latency * 0.001)
            {
                outBuffer.RemoveAt(0);
                pac.Invoke();
            }
            else
            {
                break;
            }
        }
    }

    public static void Invoke_Server_Func_Unreliable(NetworkedBehaviour Target, string FunctionName, object[] args)
    {
        NetworkedBehaviour ServerTarget = Target.GetServerInstance();

        if (ServerTarget == null)
        {
            Debug.LogError("Server instance not found for object " + Target);
            return;
        }

        NetworkPacket packet = new NetworkPacket(ServerTarget, FunctionName, args, false);

        if (ServerTarget == Target) //Invoke function immediately on server
        {
            packet.Invoke();
            return;
        }

        Instance.BufferIncomingPacket(packet);

    }

    public static void Invoke_Client_Func_Unreliable(NetworkedBehaviour Target, string FunctionName, object[] args)
    {
        NetworkedBehaviour ClientTarget = Target.GetClientInstance();

        if (ClientTarget == null)
        {
            Debug.LogError("Client instance not found for object " + Target);
            return;
        }

        NetworkPacket packet = new NetworkPacket(ClientTarget, FunctionName, args, false);

        if (ClientTarget == Target) //Invoke function immediately on owning client
        {
            packet.Invoke();
            return;
        }

        Instance.BufferOutgoingPacket(packet);
    }

    public static void Invoke_Server_Func_Reliable(NetworkedBehaviour Target, string FunctionName, object[] args)
    {
        NetworkedBehaviour ServerTarget = Target.GetServerInstance();

        if (ServerTarget == null)
        {
            Debug.LogError("Server instance not found for object " + Target);
            return;
        }

        NetworkPacket packet = new NetworkPacket(ServerTarget, FunctionName, args, true);

        if (ServerTarget == Target) //Invoke function immediately on server
        {
            packet.Invoke();
            return;
        }

        Instance.BufferIncomingPacket(packet);

    }

    public static void Invoke_Client_Func_Reliable(NetworkedBehaviour Target, string FunctionName, object[] args)
    {
        NetworkedBehaviour ClientTarget = Target.GetClientInstance();

        if (ClientTarget == null)
        {
            Debug.LogError("Client instance not found for object " + Target);
            return;
        }

        NetworkPacket packet = new NetworkPacket(ClientTarget, FunctionName, args, true);

        if (ClientTarget == Target) //Invoke function immediately on owning client
        {
            packet.Invoke();
            return;
        }

        Instance.BufferOutgoingPacket(packet);

    }

}

public class NetworkPacket
{
    //meta
    public bool bIsReliable = false;
    public double timestamp;

    //invocation info
    public NetworkedBehaviour target;
    public string methodName;
    public object[] args;

    public NetworkPacket(NetworkedBehaviour _target, string _methodName, object[] _args, bool _bIsReliable = false)
    {
        target = _target;
        methodName = _methodName;
        args = _args;
        bIsReliable = _bIsReliable;

        timestamp = Time.timeSinceLevelLoadAsDouble;
    }

    public void Invoke()
    {
        MethodInfo Func = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (Func == null)
        {
            Debug.LogError("Function " + methodName + " not found on object of type " + target.GetType());
            return;
        }

        Func.Invoke(target, args);
    }
}
