using UnityEngine;
using UnityEngine.AI;

public class AStarAgent : MonoBehaviour
{
    private NavMeshAgent agent;
        private float episodeStartTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject goalGO = GameObject.FindGameObjectWithTag("Goal");
        if (goalGO != null)
        {
            agent.SetDestination(goalGO.transform.position);
        }
        else
        {
            Debug.LogError("No GameObject tagged 'Goal' found!");
        }

        // Start the timer for the episode
        episodeStartTime = Time.time;
    }

    void Update()
    {
        // (Optional) Log when we arrive:
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
                // We have arrived at the destination
            float timeTaken = Time.time - episodeStartTime;
            Debug.Log($"[AStarAgent] Reached goal in {Time.timeSinceLevelLoad:0.00} seconds.");
            enabled = false; // stop checking once we’ve arrived
        }
    }
}