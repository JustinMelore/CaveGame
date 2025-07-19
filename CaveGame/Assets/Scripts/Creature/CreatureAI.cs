using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CreatureAI : MonoBehaviour
{
    public float randomRange = 35.0f;
    public float footRange = 15.0f;
    private int mode;
    private bool tracking = false;
    NavMeshAgent agent;
    public PlayerController playerC;
    public Transform playerT;
    private Behavior currentBehavior;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed;

    [Header("Digging Settings")]
    [SerializeField] private float digTimer;
    [SerializeField] private float digSearchTime;
    [SerializeField] private float soundThreshold;
    private float currentDigTimer;
    private float currentDigSearchTimer;
    private float currentSoundLevel;
    private DiggingSpawningPoint diggingSpawnPoint;

    public static event Action OnPlayerCaught;

    private enum Behavior
    {
        TRACKING,
        CHASING,
        DIGGING,
        INACTIVE
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mode = 0;
        currentBehavior = Behavior.TRACKING;
        currentDigTimer = 0f;
        diggingSpawnPoint = FindFirstObjectByType<DiggingSpawningPoint>();
        //SwitchCurrentBehavior(Behavior.CHASING);
    }

    void Update()
    {
        switch(currentBehavior)
        {
            case Behavior.TRACKING:
                HandleTrackingBehavior();
                break;
            case Behavior.CHASING:
                HandleChasingBehavior();
                break;
            case Behavior.DIGGING:
                HandleDiggingBehavior();
                break;
        }
    }

    private void HandleTrackingBehavior()
    {
        //Debug.Log("Player xv = " + playerC.getPV().x);
        //Debug.Log("Player yv = " + playerC.getPV().y);
        //Debug.Log("Player zv = " + playerC.getPV().z);
        currentDigTimer += Time.deltaTime;
        if (currentDigTimer >= digTimer) SwitchCurrentBehavior(Behavior.DIGGING);
        if (playerC.getPV().x != 0 || playerC.getPV().z != 0) //don't know how to include jumping if y velocity is always -2
        {
            if (playerC.getROS().volume != 0 || playerC.getRSS().volume != 0)
            {
                mode = 3;
            }
            else
            {
                mode = 1;
            }
        }
        else if (playerC.getROS().volume != 0 || playerC.getRSS().volume != 0)
        {
            mode = 2;
        }
        else
        {
            mode = 0;
        }
        if (mode == 0)
        {
            //Debug.Log("Mode is " + mode);
            agent.speed = 1.5f;
            Random();
        }
        else if (mode == 1)
        {
            //Debug.Log("Mode is " + mode);
            agent.speed = 1.5f;
            float willIt = UnityEngine.Random.Range(0, 10);
            if (willIt < 5) //final odds are yet to be decided
            {
                Track();
            }
            else
            {
                Random();
            }
        }
        else if (mode == 2)
        {
            //Debug.Log("Mode is " + mode);
            agent.speed = 3f;
            float willIt = UnityEngine.Random.Range(0, 10);
            if (willIt < 5) //final odds are yet to be decided
            {
                Track();
            }
            else
            {
                Random();
            }
        }
        if (mode == 3)
        {
            //Debug.Log("Mode is " + mode);
            agent.speed = 4.5f;
            float willIt = UnityEngine.Random.Range(0, 10);
            if (willIt < 5) //final odds are yet to be decided
            {
                Track();
            }
            else
            {
                Random();
            }
        }
        if (agent.remainingDistance < 0.1f)
        {
            tracking = false;
        }
        if (Vector3.Distance(transform.position, playerT.position) <= footRange && mode != 0) SwitchCurrentBehavior(Behavior.CHASING);
    }


    private void HandleChasingBehavior()
    {
        agent.SetDestination(playerT.position);
    }

    private void HandleDiggingBehavior()
    {
        //TODO Add digging animation and noise cue
        currentDigSearchTimer += Time.deltaTime;
        if (currentDigSearchTimer >= digSearchTime)
        {
            Debug.Log("Player not found while digging");
            agent.isStopped = false;
            SwitchCurrentBehavior(Behavior.TRACKING);
        }
        int currentNoiseAmount = 0;
        if (playerC.getPV().x != 0 || playerC.getPV().z != 0) currentNoiseAmount += 1;
        if (playerC.getROS().volume != 0 || playerC.getRSS().volume != 0) currentNoiseAmount += 2;
        currentSoundLevel += currentNoiseAmount * Time.deltaTime;
        if(currentSoundLevel >= soundThreshold)
        {
            Debug.Log($"Sound level reached {soundThreshold}: Creature found player");
            transform.position = diggingSpawnPoint.transform.position;
            agent.isStopped = false;
            SwitchCurrentBehavior(Behavior.CHASING);
        }

    }

    private void SwitchCurrentBehavior(Behavior newBehavior)
    {
        if(newBehavior == Behavior.DIGGING)
        {
            currentDigSearchTimer = 0f;
            currentDigTimer = 0f;
            currentSoundLevel = 0f;
            agent.isStopped = true;
        }
        if (newBehavior == Behavior.CHASING) agent.speed = chaseSpeed;
        currentBehavior = newBehavior;
        Debug.Log($"Currently {newBehavior}");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collided with {other}");
        OnPlayerCaught?.Invoke();
        SwitchCurrentBehavior(Behavior.INACTIVE);
    }

    void Random()
    {
        if (agent.pathPending || !agent.isOnNavMesh || agent.remainingDistance > 0.1f)
            return;

        //have sniffing animation between each new destination
        Vector3 unV = UnityEngine.Random.insideUnitCircle;
        float rv = UnityEngine.Random.Range(0, randomRange);
        agent.destination = (rv * unV) + (footRange * unV);
    }

    void Track()
    {
        if (agent.pathPending || !agent.isOnNavMesh || (tracking == true && agent.remainingDistance > 0.1f))
            return;

        //Vector2 pPos = new Vector2(player.getController().center.x, player.getController().center.z);
        //Vector2 cPos = new Vector2(GetComponent<Transform>().position.x, GetComponent<Transform>().position.z);
        agent.destination = playerT.position;
        tracking = true;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
    //    Gizmos.DrawSphere(transform.position, footRange);
    //}
}
