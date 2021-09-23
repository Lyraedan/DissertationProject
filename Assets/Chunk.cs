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
        GenerateColours();
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
        GenerateColours();
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

                float avg = noise + noise2 + noise3 + noise4 / 4;

                int pixelIndex = (int)z * MeshGenerator.resolution + (int)x;
                pixels[pixelIndex] = new Color(avg, avg, avg);
                //generator.colorSettings.material.SetVector("_heights", new Vector4(noise, noise2, noise3, noise4));
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
        float sample = 0;
        float offset = 5000.0f;
        float amplitude = 0;
        float sampleIncrement = 0;

        float chunkX = transform.position.x * MeshGenerator.resolution;
        float chunkZ = transform.position.z * MeshGenerator.resolution;

        for (int i = 0; i < noiseSettings.Length; i++)
        {
            if (noiseSettings[i].enabled)
            {
                for (int j = 0; j < noiseSettings[i].interations; j++)
                {
                    float noiseX = ((offset + chunkX + x) * noiseSettings[i].roughness) / MeshGenerator.resolution;
                    float noiseZ = ((offset + chunkZ + z) * noiseSettings[i].roughness) / MeshGenerator.resolution;
                    switch(noiseSettings[i].noiseType)
                    {
                        case NoiseSettings.NoiseType.Perlin:
                            sample += Mathf.PerlinNoise(noiseX, noiseZ) * noiseSettings[i].frequancy;
                            break;
                        case NoiseSettings.NoiseType.Perlin_Abs:
                            sample += 1f - Mathf.Abs(Mathf.Sin(Mathf.PerlinNoise(noiseX, noiseZ))) * noiseSettings[i].frequancy;
                            break;
                        case NoiseSettings.NoiseType.Simplex:
                            sample += SimplexNoise.SimplexNoise.Generate(noiseX, noiseZ) * noiseSettings[i].frequancy;
                            break;
                        default:
                            // Default algorithm is perlin
                            sample += Mathf.PerlinNoise(noiseX, noiseZ) * noiseSettings[i].frequancy;
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

    public void GenerateColours()
    {
        UpdateMinMax();
        UpdateColors();
    }

    void UpdateMinMax()
    {
        generator.colorSettings.material.SetVector("_minMax", new Vector4(minMax.Min, minMax.Max));
    }

    void UpdateColors()
    {
        Color[] colors = new Color[generator.colorSettings.textureResolution];
        for (int i = 0; i < generator.colorSettings.textureResolution; i++) {
            colors[i] = generator.colorSettings.gradient.Evaluate(i / (generator.colorSettings.textureResolution - 1f));
        }
        generator.colorSettings.texture.SetPixels(colors);
        generator.colorSettings.texture.Apply();
        generator.colorSettings.material.SetTexture("_texture", generator.colorSettings.texture);
    }


}
