using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingAreaSpawner : MonoBehaviour
{
    public GameObject trainingAreaPrefab;
    public int gridSize = 4; // 4x4 grid = 16 agents
    public float spacing = 50f;

    void Start()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 pos = new Vector3(x * spacing, 0, z * spacing);
                Instantiate(trainingAreaPrefab, pos, Quaternion.identity);
            }
        }
    }
}