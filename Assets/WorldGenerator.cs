using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public float viewSize = 1f;
    public GameObject chunkPrefab;
    public Camera cam;

    public Dictionary<string, GameObject> chunks = new Dictionary<string, GameObject>();

    [Header("Editor")]
    public bool displayChunkDetails = false;

    // Start is called before the first frame update
    void Start()
    {
        SpawnChunk(0, 0);
        //GenerateChunkIfWeNeedTo();
    }

    private void Update()
    {
        // Unrender chunks that are far away from the camera
        /*
        float camX = cam.transform.position.x / (MeshGenerator.tileSize * MeshGenerator.resolution);
        float camY = cam.transform.position.y / (MeshGenerator.tileSize * MeshGenerator.resolution);
        float camZ = cam.transform.position.z / (MeshGenerator.tileSize * MeshGenerator.resolution);

        for (int i = 0; i < chunks.Count; i++)
        {
            bool doEnable = MathUtils.DistanceFrom(chunks.Values.ElementAt(i).transform.position, new Vector3(camX, camY, camZ)) < (3 * MeshGenerator.resolution);
            chunks.Values.ElementAt(i).SetActive(doEnable);
        }
        */
        //GenerateChunkIfWeNeedTo();
    }

    void SpawnChunk(float x, float z)
    {
        if (ChunkExistsAt(x, z))
            return;

        GameObject spawned = Instantiate(chunkPrefab, new Vector3(x, 0, z), Quaternion.identity);
        spawned.transform.SetParent(transform);
        spawned.name = $"{x}_{z}";
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
                    SpawnChunk(x * MeshGenerator.resolution / 2, z * MeshGenerator.resolution / 2);
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
        return Mathf.Round(cam.transform.position.x / MeshGenerator.resolution);
    }

    float ChunkZ()
    {
        return Mathf.Round(cam.transform.position.z / MeshGenerator.resolution);
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < chunks.Count; i++)
        {
            if (displayChunkDetails)
                UnityEditor.Handles.Label(chunks.ElementAt(i).Value.transform.position, "(" + chunks.ElementAt(i).Value.transform.position.x + ", " + chunks.ElementAt(i).Value.transform.position.z + " : " + i + ")");
        }
    }
}
