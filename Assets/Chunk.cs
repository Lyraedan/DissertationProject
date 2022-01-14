using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public MeshGenerator generator;
    public NoiseSettings[] noiseSettings = new NoiseSettings[1];

    public Vector3 worldSpace = Vector3.zero;

    public Texture2D noiseTexture;
    private Color[] pixels;

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
        Erode();
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
        Erode();
        ApplyFoliage();
        //for (int i = 0; i < resolution * resolution; i++)
        //{
        //    generator.RecalculateNormalAt(i);
        //}
        GenerateColours();
        generator.Refresh();
    }

    public void ApplyNoise()
    {
        // Generate the heightmap
        for (int z = 0; z < MeshGenerator.resolution.y; z++)
        {
            for(int x = 0; x < MeshGenerator.resolution.x; x++)
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
    public void Erode()
    {
        /*
        int chunkInterationCount = 1;
        float delta = 0;
        for(int z = 0; z < MeshGenerator.resolution.y; z++)
        {
            for(int x = 0; x < MeshGenerator.resolution.x; x++)
            {
                for(int i = 0; i < chunkInterationCount; i++)
                {
                    float tl = CalculateHeight(x, z);
                    float tr = CalculateHeight(x + 1, z);
                    float bl = CalculateHeight(x, z + 1);
                    float br = CalculateHeight(x + 1, z + 1);

                    var t = delta *= Time.deltaTime;
                    tl -= t;
                    tr -= t;
                    bl -= t;
                    br -= t;
                    int pixelIndex = (int)z * MeshGenerator.resolution.x + (int)x;

                    generator.UpdateHeights(pixelIndex, new float[] { tl, tr, bl, br });
                }
            }
        }
        */
    }

    public void ApplyFoliage()
    {
        // Spawn trees, grass, rocks etc
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
                    switch(noiseSettings[i].noiseType)
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
        for (int i = 0; i < generator.colorSettings.textureResolution; i++) {
            colors[i] = generator.colorSettings.gradient.Evaluate(i / (generator.colorSettings.textureResolution - 1f));
        }
        generator.colorSettings.texture.SetPixels(colors);
        generator.colorSettings.texture.Apply();
        generator.colorSettings.material.SetTexture("_texture", generator.colorSettings.texture);
    }


}
