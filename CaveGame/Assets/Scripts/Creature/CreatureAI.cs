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

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mode = 0;
    }

    void Update()
    {
        //Debug.Log("Player xv = " + playerC.getPV().x);
        //Debug.Log("Player yv = " + playerC.getPV().y);
        //Debug.Log("Player zv = " + playerC.getPV().z);
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
            Debug.Log("Mode is " + mode);
            agent.speed = 1.5f;
            Random();
        }
        else if (mode == 1)
        {
            Debug.Log("Mode is " + mode);
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
            Debug.Log("Mode is " + mode);
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
            Debug.Log("Mode is " + mode);
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
}
