using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public MeshGenerator generator;
    public NoiseSettings[] noiseSettings = new NoiseSettings[1];

    public Texture2D noiseTexture;
    private Color[] pixels;
    private MinMax minMax;

    public int resolution {
        get {
            return MeshGenerator.resolution;
        }
    }

    public void Initialize()
    {
        minMax = new MinMax();
        generator = GetComponent<MeshGenerator>();
        generator.Initialize();

        noiseTexture = new Texture2D(MeshGenerator.resolution, MeshGenerator.resolution);
        pixels = new Color[noiseTexture.width * noiseTexture.height];
    }

    public void GenerateChunk()
    {
        generator.GeneratePlane(transform.position.x, transform.position.z);
        ApplyNoise();
        Erode();
        ApplyFoliage();
        for(int i = 0; i < resolution * resolution; i++)
        {
            generator.RecalculateNormalAt(i);
        }
        generator.Refresh();
    }

    public void UpdateChunk()
    {
        ApplyNoise();
        Erode();
        ApplyFoliage();
        for (int i = 0; i < resolution * resolution; i++)
        {
            generator.RecalculateNormalAt(i);
        }
        generator.Refresh();
    }

    public void ApplyNoise()
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
                //generator.colorSettings.material.SetVector("_heights", new Vector4(noise, noise2, noise3, noise4));
                generator.UpdateHeights(pixelIndex, new float[] { noise, noise2, noise3, noise4 });
            }
        }

        noiseTexture.SetPixels(pixels);
        noiseTexture.Apply();
        generator.meshRenderer.material.SetTexture("_MainTex", noiseTexture);
        //generator.colorSettings.material.SetTexture("_heightmap", noiseTexture);
    }

    public void Erode()
    {

    }

    public void ApplyFoliage()
    {

    }

    float CalculateHeight(float x, float z)
    {
        float sample = 0;
        float offset = 5000.0f;
        float amplitude = 0;
        float sampleIncrement = 0;
        for(int i = 0; i < noiseSettings.Length; i++)
        {
            if (noiseSettings[i].enabled)
            {
                for (int j = 0; j < noiseSettings[i].interations; j++)
                {
                    float noiseX = ((offset + transform.position.x + x) / MeshGenerator.resolution + sampleIncrement) * noiseSettings[i].roughness;
                    float noiseZ = ((offset + transform.position.z + z) / MeshGenerator.resolution + sampleIncrement) * noiseSettings[i].roughness;
                    switch(noiseSettings[i].type)
                    {
                        case NoiseSettings.NoiseType.Hilly:
                            sample += Mathf.PerlinNoise(noiseX, noiseZ) * noiseSettings[i].frequancy;
                            break;
                        case NoiseSettings.NoiseType.Mountainous:
                            sample += 1f - Mathf.Abs(Mathf.PerlinNoise(noiseX, noiseZ) * noiseSettings[i].frequancy);
                            break;
                    }
                    sampleIncrement += amplitude;
                    amplitude *= noiseSettings[i].persistance;
                }
                sample = Mathf.Max(0, sample - noiseSettings[i].minValue);
                sample *= noiseSettings[i].strength;
            }
        }
        minMax.AddValue(sample);
        return sample;
    }
}
