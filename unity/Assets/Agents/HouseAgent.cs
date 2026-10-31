// File: Assets/Scripts/HouseAgent.cs

using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(Rigidbody))]
public class HouseAgent : Agent
{
    [Header("References")]
    [Tooltip("Drag your exit‐door GameObject (tagged “Goal”) here.")]
    public Transform goal;
    [Tooltip("LayerMask for walls & obstacles (must be set to the “Obstacle” layer).")]
    public LayerMask obstacleMask;

    [Header("Movement Settings")]
    [Tooltip("Forward/backward speed.")]
    public float moveSpeed = 2f;
    [Tooltip("Rotation speed (degrees/sec).")]
    public float rotateSpeed = 100f;
    [Tooltip("Max distance for each raycast observation.")]
    public float maxRayDistance = 10f;

    [Header("Episode Settings")]
    [Tooltip("How many FixedUpdate steps before timing out.")]
    public int maxStepLimit = 500;
    private int currentStepCount = 0;

    // For reward shaping
    private Vector3 startPos;
    private Quaternion startRot;
    private Rigidbody rBody;
    private float lastDistToGoal;

    // For action smoothing
    private Vector2 lastRawAction = Vector2.zero;
    [Tooltip("Interpolation factor [0..1] for smoothing between last and new action. 0 means NO smoothing.")]
    [Range(0f, 1f)]
    public float smoothingFactor = 0f;

    void Awake()
    {
        // Cache Rigidbody and initial transform
        rBody = GetComponent<Rigidbody>();
        rBody.useGravity = true;
        rBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        startPos = transform.position;
        startRot = transform.rotation;

        // Ensure a DecisionRequester is attached so the agent requests every step
        var dr = GetComponent<Unity.MLAgents.DecisionRequester>();
        if (dr == null)
        {
            dr = gameObject.AddComponent<Unity.MLAgents.DecisionRequester>();
        }
        dr.DecisionPeriod = 1; // request a new action every physics step
        dr.TakeActionsBetweenDecisions = true;
    }

    public override void Initialize()
    {
        if (goal != null)
        {
            lastDistToGoal = Vector3.Distance(transform.position, goal.position);
        }
    }

    public override void OnEpisodeBegin()
    {
        // Reset position & orientation
        transform.position = startPos;
        transform.rotation = startRot;
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;

        // Reset counters and shaping metric
        currentStepCount = 0;
        if (goal != null)
        {
            lastDistToGoal = Vector3.Distance(transform.position, goal.position);
        }

        // Clear lastAction
        lastRawAction = Vector2.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (goal == null)
        {
            for (int i = 0; i < 8; i++)
                sensor.AddObservation(0f);
            return;
        }

        // 1) Direction & normalized distance to goal (3 floats)
        Vector3 toGoal = (goal.position - transform.position).normalized;
        float distToGoal = Vector3.Distance(transform.position, goal.position);
        float normDist = Mathf.Min(distToGoal / maxRayDistance, 1f);
        sensor.AddObservation(toGoal.x); // obs 0
        sensor.AddObservation(toGoal.z); // obs 1
        sensor.AddObservation(normDist); // obs 2

        Debug.DrawLine(transform.position, goal.position, Color.red);

        // 2) Five‐ray obstacle distances (5 floats)
        float[] angles = { -90f, -45f, 0f, 45f, 90f };
        foreach (float ang in angles)
        {
            Vector3 dir = Quaternion.Euler(0f, ang, 0f) * transform.forward;
            Debug.DrawRay(transform.position, dir * maxRayDistance, Color.green);
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, maxRayDistance, obstacleMask))
            {
                sensor.AddObservation(hit.distance / maxRayDistance);
            }
            else
            {
                sensor.AddObservation(1f);
            }
        }
        // Total obs = 3 + 5 = 8
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 1) Step limit check
        currentStepCount++;
        if (currentStepCount > maxStepLimit)
        {
            AddReward(-0.5f);
            float timeTakenSim = currentStepCount * Time.fixedDeltaTime;
            Debug.Log($"[HouseAgent] Timed out after {currentStepCount} steps, ~{timeTakenSim:0.00} simulated seconds.");
            EndEpisode();
            return;
        }

        // 2) Read raw continuous actions
        float rawMove = actions.ContinuousActions[0]; // in [-1,1]
        float rawTurn = actions.ContinuousActions[1]; // in [-1,1]

        // 3) Smooth actions (if smoothingFactor > 0) or else use raw directly
        Vector2 rawAction = new Vector2(rawMove, rawTurn);
        Vector2 smoothed;
        if (smoothingFactor <= 0f)
        {
            // No smoothing
            smoothed = rawAction;
        }
        else
        {
            // Interpolate between lastRawAction and rawAction
            smoothed = Vector2.Lerp(lastRawAction, rawAction, smoothingFactor);
        }
        lastRawAction = smoothed;

        float move = Mathf.Clamp(smoothed.x, -1f, 1f);
        float turn = Mathf.Clamp(smoothed.y, -1f, 1f);

        // 4) Apply movement & rotation
        transform.Rotate(0f, turn * rotateSpeed * Time.deltaTime, 0f);
        Vector3 forward = transform.forward * move * moveSpeed * Time.deltaTime;
        rBody.MovePosition(rBody.position + forward);

        // 5) Reward shaping: positive if moving closer to goal (stronger weight)
        if (goal != null)
        {
            float currentDist = Vector3.Distance(transform.position, goal.position);
            float deltaDist = lastDistToGoal - currentDist;
            AddReward(deltaDist * 1.0f);
            lastDistToGoal = currentDist;
        }

        // 6) Larger time penalty per step
        AddReward(-0.005f);

        // 7) Check for reaching the goal
        if (goal != null && Vector3.Distance(transform.position, goal.position) < 1.5f)
        {
            AddReward(+1.0f);

            float timeTakenSim = currentStepCount * Time.fixedDeltaTime;
            Debug.Log($"[HouseAgent] Reached goal in {currentStepCount} steps, ~{timeTakenSim:0.00} simulated seconds.");

            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var cont = actionsOut.ContinuousActions;
        cont[0] = Input.GetKey(KeyCode.W) ?  1f :
                  Input.GetKey(KeyCode.S) ? -1f : 0f;
        cont[1] = Input.GetKey(KeyCode.A) ? -1f :
                  Input.GetKey(KeyCode.D) ?  1f : 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") ||
            collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-0.2f);
        }
    }
}
