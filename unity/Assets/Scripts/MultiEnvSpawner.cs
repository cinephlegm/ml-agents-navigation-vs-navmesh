using UnityEngine;

[AddComponentMenu("ML-Agents/Utilities/MultiEnvSpawner")]
public class MultiEnvSpawner : MonoBehaviour
{
    [Header("Prefab & Layout")]
    [Tooltip("Drag in your house-environment prefab (which contains exactly one agent, walls, obstacles, and the Goal).")]
    public GameObject environmentPrefab;

    [Tooltip("Total number of environments (i.e. agents) to spawn in the scene.")]
    public int numEnvironments = 4;

    [Tooltip("How many environments to put in each row (number of columns in the grid).")]
    public int environmentsPerRow = 2;

    [Tooltip("Spacing (in X and Z) between the centers of each environment clone.")]
    public float spacing = 40f;

    void Start()
    {
        if (environmentPrefab == null)
        {
            Debug.LogError("[MultiEnvSpawner] Please assign a prefab in the Inspector.");
            return;
        }

        // Determine how many rows we need:
        int rows = Mathf.CeilToInt(numEnvironments / (float)environmentsPerRow);

        int spawnedCount = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < environmentsPerRow; col++)
            {
                if (spawnedCount >= numEnvironments) break;

                // Calculate offset so that each clone is in its own 2D grid cell
                Vector3 offset = new Vector3(col * spacing, 0f, row * spacing);

                // Instantiate the prefab at position + offset
                GameObject instance = Instantiate(
                    environmentPrefab,
                    transform.position + offset,
                    Quaternion.identity
                );

                // (Optional) rename the clone to keep the Hierarchy tidy
                instance.name = environmentPrefab.name + "_" + spawnedCount;

                spawnedCount++;
            }
        }

        // Disable the spawner object itself so it doesn't interfere with training
        gameObject.SetActive(false);
    }
}
