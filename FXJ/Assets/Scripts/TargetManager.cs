using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetManager : MonoBehaviour
{
    [SerializeField] TargetBehaviour[] targets;

    public static TargetManager instance;
    public UnityAction OnTargetKilled = delegate { };
    public bool bIsGameActive = true;

    private Queue<int> sequence = new Queue<int>();
    private int[,] pseudoRandomOrder = new int[,] { {0, 2 ,1, 2, 0, 1}};


    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        OnTargetKilled += DeployNextTargetFunc;
    }

    private void Start()
    {
        StartSequence(0);
    }

    private void DeployNextTargetFunc()
    {
        StartCoroutine(DeployNextTarget());
    }

    private IEnumerator DeployNextTarget()
    {
        if (sequence.Count <= 0)
        {
            bIsGameActive = false;
            yield break;
        }

        yield return new WaitForSeconds(1.0f);

        targets[sequence.Dequeue()].Deploy();

    }
    public void StartSequence(int seed)
    {
        
        for (int i = 0; i < pseudoRandomOrder.Length; i++)
        {
            sequence.Enqueue(pseudoRandomOrder[seed, i]);
        }

        DeployNextTargetFunc();

    }

}
