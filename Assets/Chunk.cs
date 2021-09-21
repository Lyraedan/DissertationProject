using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    private MeshGenerator generator;

    public int resolution {
        get {
            return MeshGenerator.resolution;
        }
    }

    public void Initialize()
    {
        generator = GetComponent<MeshGenerator>();
        generator.Initialize();
    }

    public void GenerateChunk()
    {
        generator.GeneratePlane(transform.position.x, transform.position.z);
        ApplyNoise(1);
        Erode();
        ApplyFoliage();
        for(int i = 0; i < resolution * resolution; i++)
        {
            generator.RecalculateNormalAt(i);
        }
        generator.Refresh();
    }

    void ApplyNoise(int layers)
    {
        float offset = 0f;
        for(int z = 0; z < resolution; z++)
        {
            for(int x = 0; x < resolution; x++)
            {
                float noiseX = (offset + transform.position.x + x) / resolution;
                float noiseZ = (offset + transform.position.z + z) / resolution;

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
