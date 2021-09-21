using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameObject chunkPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for(int x = 0; x < 300; x++)
        {
            for(int z = 0; z < 300; z++)
            {
                SpawnChunk(x, z);
            }
        }
    }

    void SpawnChunk(float x, float z)
    {
        GameObject spawned = Instantiate(chunkPrefab, new Vector3(x, 0f, z), Quaternion.identity);
        Chunk chunk = spawned.GetComponent<Chunk>();
        chunk.Initialize();
        chunk.GenerateChunkAt(new Vector3(x * chunk.resolution, 0f, z * chunk.resolution));
    }

}
