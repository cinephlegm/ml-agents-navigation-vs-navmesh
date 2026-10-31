using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors; // Required for RayPerceptionSensor (though it collects automatically)
using System.Collections.Generic; // For randomizing target positions

public class MazeNavigationAgent : Agent
{
    public Transform target;
    public float moveAmount = 1.0f; // How far to move in one "Move Forward" action
    public float turnAmount = 90.0f; // How many degrees to turn for "Turn Left/Right"

    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;

    // Optional: List of possible spawn points for the target to vary episodes
    public List<Transform> targetSpawnPoints;
    // Optional: List of possible spawn points for the agent
    public List<Transform> agentSpawnPoints;

    // For reward shaping (optional but can help)
    private float previousDistanceToTarget;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.localPosition; // Store initial local position
        startRotation = transform.localRotation; // Store initial local rotation

        if (rb != null)
        {
            rb.freezeRotation = true; // Often good for simple grid movement, adjust if needed
        }
    }

    public override void OnEpisodeBegin()
    {
        // Reset Agent
        if (agentSpawnPoints != null && agentSpawnPoints.Count > 0)
        {
            transform.position = agentSpawnPoints[Random.Range(0, agentSpawnPoints.Count)].position;
            // Optionally, also set a random start rotation if your maze isn't grid-aligned for starting positions
            // transform.rotation = Quaternion.Euler(0, Random.Range(0, 4) * 90f, 0);
        }
        else
        {
            transform.localPosition = startPosition;
            transform.localRotation = startRotation;
        }
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }


        // Reset Target Position (Crucial for generalization in a maze)
        if (targetSpawnPoints != null && targetSpawnPoints.Count > 0)
        {
            target.position = targetSpawnPoints[Random.Range(0, targetSpawnPoints.Count)].position;
        }
        // Else, ensure the target is in a fixed, known good position if not randomizing

        // Update distance for reward shaping
        previousDistanceToTarget = Vector3.Distance(transform.position, target.position);
    }

    // Observations are primarily handled by RayPerceptionSensor3D component.
    // You can add custom observations if needed.
    public override void CollectObservations(VectorSensor sensor)
    {
        // Example: Normalized direction to target (can be helpful)
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        sensor.AddObservation(directionToTarget); // 3 floats

        // Example: Distance to target (can be helpful)
        // sensor.AddObservation(Vector3.Distance(transform.position, target.position) / MAX_MAZE_DISTANCE); // Normalize this!

        // The RayPerceptionSensor3D will add its own observations automatically.
        // Ensure your Behavior Parameters > Space Size reflects these custom observations + RayPerceptionSensor's observations.
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int discreteAction = actions.DiscreteActions[0]; // Assuming one branch of discrete actions

        Vector3 currentPosition = transform.position;

        // Action Definitions:
        // 0: Move Forward
        // 1: Turn Left (e.g., -90 degrees)
        // 2: Turn Right (e.g., +90 degrees)
        // (Optional 3: Do Nothing / Wait)

        switch (discreteAction)
        {
            case 0: // Move Forward
                rb.MovePosition(transform.position + transform.forward * moveAmount);
                break;
            case 1: // Turn Left
                transform.Rotate(transform.up, -turnAmount);
                break;
            case 2: // Turn Right
                transform.Rotate(transform.up, turnAmount);
                break;
            // case 3: // Do Nothing (optional)
            //     break;
        }

        // --- REWARDS ---

        // 1. Living/Step Penalty (encourages efficiency)
        AddReward(-1f / MaxStep); // MaxStep is a property of Agent component, set it in Inspector or it defaults

        // 2. Reward Shaping: Getting Closer to Target (Optional, use with care)
        float currentDistanceToTarget = Vector3.Distance(transform.position, target.position);
        if (currentDistanceToTarget < previousDistanceToTarget)
        {
            AddReward(0.01f); // Small positive reward for reducing distance
        }
        previousDistanceToTarget = currentDistanceToTarget;
    }

    // For testing with keyboard input
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 3; // Default to "Do Nothing" if you have that action

        if (Input.GetKeyDown(KeyCode.W))
        {
            discreteActions[0] = 0; // Move Forward
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            discreteActions[0] = 1; // Turn Left
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            discreteActions[0] = 2; // Turn Right
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            SetReward(1.0f); // Big positive reward for reaching the goal
            Debug.Log("Goal Reached!");
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Obstacle"))
        {
            SetReward(-0.5f); // Negative reward for hitting a wall
            Debug.Log("Hit a Wall!");
            EndEpisode(); // End episode on hitting a wall to speed up learning invalid paths
        }
    }
}