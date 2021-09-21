using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    private MeshGenerator generator;
    public Vector3 chunkPosition = Vector3.zero;

    public int resolution {
        get {
            return generator.resolution;
        }
    }

    public void Initialize()
    {
        generator = GetComponent<MeshGenerator>();
        generator.Initialize();
    }

    public void GenerateChunkAt(Vector3 position)
    {
        this.chunkPosition = position;
        generator.GeneratePlane(position.x, position.z);
        ApplyNoise(1);
        Erode();
        ApplyFoliage();
        for(int i = 0; i < generator.resolution * generator.resolution; i++)
        {
            generator.RecalculateNormalAt(i);
        }
        generator.Refresh();
    }

    void ApplyNoise(int layers)
    {
        float offset = 0f;
        for(int z = 0; z < generator.resolution; z++)
        {
            for(int x = 0; x < generator.resolution; x++)
            {
                float noiseX = (offset + chunkPosition.x + x) / generator.resolution;
                float noiseZ = (offset + chunkPosition.z + z) / generator.resolution;

                float noise = 0;
                float amplitude = 0.1f;
                for(int i = 0; i < layers; i++)
                {
                    noise += Mathf.PerlinNoise(noiseX * amplitude, noiseZ * amplitude);
                    amplitude++;
                }
                Debug.Log("Noise: " + noise);
                generator.UpdateVerticeY(x, z, noise);
            }
        }
    }

    void Erode()
    {

    }

    void ApplyFoliage()
    {

    }
}
