using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chunk : MonoBehaviour
{
    public MeshGenerator generator;
    public NoiseSettings[] noiseSettings = new NoiseSettings[1];

    [HideInInspector] public Vector3 worldSpace = Vector3.zero;

    [HideInInspector] public Texture2D noiseTexture;
    private Color[] pixels;

    [Header("Erosion")]
    public float speed = 3f;
    public int numberOfIterations = 100;
    public float iterationScale = 0.5f;
    public float depositionRate = 1f;
    public float erosionRate = 1f;
    public float friction = 1f;

    public void Initialize()
    {
        generator = GetComponent<MeshGenerator>();
        generator.Initialize();

        noiseTexture = new Texture2D(MeshGenerator.resolution.x, MeshGenerator.resolution.y);
        pixels = new Color[noiseTexture.width * noiseTexture.height];
    }

    public void GenerateChunk()
    {
        generator.GeneratePlane(transform.position.x, transform.position.z);
        ApplyNoise();
        //Erode();
        ApplyFoliage();
        //for(int i = 0; i < resolution * resolution; i++)
        //{
        //    generator.RecalculateNormalAt(i);
        //}
        GenerateColours();
        generator.Refresh();
    }

    public void UpdateChunk()
    {
        ApplyNoise();
        //Erode();
        ApplyFoliage();
        //for (int i = 0; i < resolution * resolution; i++)
        //{
        //    generator.RecalculateNormalAt(i);
        //}
        GenerateColours();
        generator.Refresh();
    }

    public void Refresh()
    {
        GenerateColours();
        generator.Refresh();
    }

    public void ApplyNoise()
    {
        // Generate the heightmap
        for (int z = 0; z < MeshGenerator.resolution.y; z++)
        {
            for (int x = 0; x < MeshGenerator.resolution.x; x++)
            {
                float noise = CalculateHeight(x, z);
                float noise2 = CalculateHeight(x + 1, z);
                float noise3 = CalculateHeight(x, z + 1);
                float noise4 = CalculateHeight(x + 1, z + 1);

                float avg = noise + noise2 + noise3 + noise4 / 4;

                int pixelIndex = (int)z * MeshGenerator.resolution.x + (int)x;
                pixels[pixelIndex] = new Color(avg, avg, avg);
                //generator.colorSettings.material.SetVector("_heights", new Vector4(noise, noise2, noise3, noise4));
                generator.UpdateHeights(pixelIndex, new float[] { noise, noise2, noise3, noise4 });
            }
        }

        noiseTexture.SetPixels(pixels);
        noiseTexture.Apply();
    }

    // ref https://web.mit.edu/cesium/Public/terrain.pdf
    //https://www.idi.ntnu.no/emner/tdt03/Presentations2013/Kazakauskas_hydraulic_erosion.pdf
    // Based: https://jobtalle.com/simulating_hydraulic_erosion.html
    public void Erode(float x, float z)
    {
        Debug.Log("Eroding");
        float sediment = 0; // The amount of carried sediment

        float xp = x; // Previous X
        float zp = z; // Previous Z

        float vx = 0; // Velocity X
        float vz = 0; // Velocity Z

        for(int i = 0; i < numberOfIterations; i++)
        {
            // Get the surface normal at the current position - Sample function needs work
            var surfaceNormal = generator.GetNormalAt(x, z);
            // The surface is flat if the y normal is 1
            if (surfaceNormal.y == 1)
                break;

            // Calculate the deposition and erosion rates
            float deposit = sediment * depositionRate * surfaceNormal.y;
            float erosion = erosionRate * (1 - surfaceNormal.y) * Mathf.Min(1, i * iterationScale);

            // Change the sediment on the place
            generator.UpdateHeightAt(xp, zp, deposit - erosion);
            sediment += erosion - deposit;

            // Update velocity, previous position, current position
            vx += friction * vx + surfaceNormal.x * speed;
            vz += friction * vz + surfaceNormal.z * speed;
            xp = x;
            zp = z;
            x += vx;
            z += vz;
        }
    }

    public void ApplyFoliage()
    {
        // Spawn trees, grass, rocks etc
    }

    Vector2 CalculateGradient(float x, float z, float tr, float tl, float br, float bl)
    {
        float gx = (tr - tl) * (1 - z) + (br - bl) * z;
        float gy = (bl - tl) * (1 - x) + (br - tr) * x;
        return new Vector2(gx, gy);
    }

    float CalculateHeight(float x, float z)
    {
        float sample = 0;
        float offset = 5000.0f;
        float amplitude = 1;

        float chunkX = (worldSpace.x + worldSpace.x) + MeshGenerator.resolution.x;
        float chunkZ = (worldSpace.z + worldSpace.z) + MeshGenerator.resolution.y;

        for (int i = 0; i < noiseSettings.Length; i++)
        {
            if (noiseSettings[i].enabled)
            {
                float frequancy = noiseSettings[i].scale;
                for (int j = 0; j < noiseSettings[i].interations; j++)
                {
                    float noiseX = WorldGenerator.instance.seed + (offset + chunkX + x) * noiseSettings[i].roughness / MeshGenerator.resolution.x;
                    float noiseZ = WorldGenerator.instance.seed + (offset + chunkZ + z) * noiseSettings[i].roughness / MeshGenerator.resolution.y;
                    switch (noiseSettings[i].noiseType)
                    {
                        case NoiseSettings.NoiseType.Perlin:
                            sample += Mathf.PerlinNoise(noiseX, noiseZ) * frequancy;
                            break;
                        case NoiseSettings.NoiseType.Perlin_Abs:
                            sample += 1f - Mathf.Abs(Mathf.Sin(Mathf.PerlinNoise(noiseX, noiseZ))) * frequancy;
                            break;
                        case NoiseSettings.NoiseType.Simplex:
                            sample += SimplexNoise.SimplexNoise.Generate(noiseX, noiseZ) * frequancy;
                            break;
                        case NoiseSettings.NoiseType.Fractal:
                            sample += ((SimplexNoise.SimplexNoise.Generate(noiseX, noiseZ) * frequancy) * 2 - 1) * amplitude;
                            break;
                        default:
                            // Default algorithm is perlin
                            sample += Mathf.PerlinNoise(noiseX, noiseZ) * frequancy;
                            break;
                    }
                    amplitude *= noiseSettings[i].persistance;
                    frequancy *= noiseSettings[i].lacuarity;
                }
                sample = Mathf.Max(0, sample - noiseSettings[i].minValue);
                sample *= noiseSettings[i].strength;
            }
        }
        //WorldGenerator.instance.minMax.AddValue(sample);
        return sample;
    }

    public void GenerateColours()
    {
        UpdateMinMax();
        UpdateColors();
    }

    void UpdateMinMax()
    {
        generator.colorSettings.material.SetVector("_minMax", new Vector4(WorldGenerator.instance.minMax.Min, WorldGenerator.instance.minMax.Max));
    }

    void UpdateColors()
    {
        Color[] colors = new Color[generator.colorSettings.textureResolution];
        for (int i = 0; i < generator.colorSettings.textureResolution; i++)
        {
            colors[i] = generator.colorSettings.gradient.Evaluate(i / (generator.colorSettings.textureResolution - 1f));
        }
        generator.colorSettings.texture.SetPixels(colors);
        generator.colorSettings.texture.Apply();
        generator.colorSettings.material.SetTexture("_texture", generator.colorSettings.texture);
    }


}
