using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameObject chunkPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for(int x = 0; x < 3; x++)
        {
            for(int z = 0; z < 3; z++)
            {
                SpawnChunk(x, z);
            }
        }
    }

    void SpawnChunk(float x, float z)
    {
        GameObject spawned = Instantiate(chunkPrefab, new Vector3(x * MeshGenerator.resolution / 2, 0, z * MeshGenerator.resolution / 2), Quaternion.identity);
        Chunk chunk = spawned.GetComponent<Chunk>();
        chunk.Initialize();
        chunk.GenerateChunk();
    }

}
