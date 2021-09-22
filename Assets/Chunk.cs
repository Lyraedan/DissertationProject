using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public MeshGenerator generator;
    public float perlinMultiplier;
    public float heightMultiplier;

    public Texture2D noiseTexture;
    private Color[] pixels;

    public int resolution {
        get {
            return MeshGenerator.resolution;
        }
    }

    public void Initialize()
    {
        generator = GetComponent<MeshGenerator>();
        generator.Initialize();

        noiseTexture = new Texture2D(MeshGenerator.resolution, MeshGenerator.resolution);
        pixels = new Color[noiseTexture.width * noiseTexture.height];
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

    public void ApplyNoise(int layers)
    {
        // Generate the heightmap
        for (int z = 0; z < resolution; z++)
        {
            for(int x = 0; x < resolution; x++)
            {
                float noise = CalculateHeight(x, z);
                float noise2 = CalculateHeight(x + 1, z);
                float noise3 = CalculateHeight(x, z + 1);
                float noise4 = CalculateHeight(x + 1, z + 1);

                int pixelIndex = (int)z * MeshGenerator.resolution + (int)x;
                pixels[pixelIndex] = new Color(noise, noise2, noise3, noise4);
                generator.UpdateHeights(pixelIndex, new float[] { noise, noise2, noise3, noise4 });
            }
        }

        noiseTexture.SetPixels(pixels);
        noiseTexture.Apply();
    }

    public void Erode()
    {

    }

    public void ApplyFoliage()
    {

    }

    float CalculateHeight(float x, float z)
    {
        float noiseX = ((transform.position.x + x) / MeshGenerator.resolution) * perlinMultiplier;
        float noiseZ = ((transform.position.z + z) / MeshGenerator.resolution) * perlinMultiplier;
        float sample = Mathf.PerlinNoise(noiseX, noiseZ) * heightMultiplier;

        float noise = sample;
        return noise;

    }
}
