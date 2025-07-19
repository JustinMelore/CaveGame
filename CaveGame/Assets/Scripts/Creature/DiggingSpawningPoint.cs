using UnityEngine;
using UnityEngine.AI;

public class DiggingSpawningPoint : MonoBehaviour
{
    //[SerializeField] private float followDistance;
    private NavMeshAgent agent;
    private PlayerController player;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindFirstObjectByType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(player.transform.position);
        //if (agent.remainingDistance <= followDistance) agent.isStopped = true;
        //else agent.isStopped = false;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = new Color(0f, 1f, 0f, 0.75f);
    //    if(agent != null) Gizmos.DrawSphere(transform.position, agent.stoppingDistance);
    //}
}
