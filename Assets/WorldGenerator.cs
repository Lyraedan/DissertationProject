using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public float viewSize = 0.1f;
    public GameObject chunkPrefab;
    public Camera cam;

    public Dictionary<string, GameObject> chunks = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for(int x = 0; x < 3; x++)
        {
            for(int z = 0; z < 3; z++)
            {
                if (!ChunkExistsAt(x, z))
                {
                    SpawnChunk(x * MeshGenerator.resolution, z * MeshGenerator.resolution);
                }
            }
        }
        //GenerateChunkIfWeNeedTo();
    }

    private void Update()
    {
        //GenerateChunkIfWeNeedTo();
    }

    void SpawnChunk(float x, float z)
    {
        GameObject spawned = Instantiate(chunkPrefab, new Vector3(x, 0, z), Quaternion.identity);
        Chunk chunk = spawned.GetComponent<Chunk>();
        chunk.Initialize();
        chunk.GenerateChunk();
        chunks.Add($"{x}_{z}", spawned);
    }

    void GenerateChunkIfWeNeedTo()
    {
        float camX = ChunkX();
        float camZ = ChunkZ();

        for(float x = camX - viewSize; x <= camX + viewSize; x++)
        {
            for(float z = camZ - viewSize; z <= camZ + viewSize; z++)
            {
                if(!ChunkExistsAt(x, z))
                {
                    SpawnChunk(x * MeshGenerator.resolution, z * MeshGenerator.resolution);
                }
            }
        }
    }

    bool ChunkExistsAt(float x, float z)
    {
        return chunks.ContainsKey($"{x}_{z}");
    }

    float ChunkX()
    {
        return Mathf.Round(cam.transform.position.x / (MeshGenerator.tileSize * MeshGenerator.resolution));
    }

    float ChunkZ()
    {
        return Mathf.Round(cam.transform.position.z / (MeshGenerator.tileSize * MeshGenerator.resolution));
    }

}
