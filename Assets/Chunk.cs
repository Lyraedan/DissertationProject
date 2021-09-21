using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public MeshGenerator generator;
    public AnimationCurve scale;

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
        // increasing above resolution causes chunks to constantly generate
        for (int z = 0; z < resolution; z++)
        {
            for(int x = 0; x < resolution; x++)
            {
                float noise = CalculateHeight(x, z);

                int pixelIndex = (int)z * MeshGenerator.resolution + (int)x;
                pixels[pixelIndex] = new Color(noise, noise, noise);
                Debug.Log("Noise: " + noise + " @ index: " + pixelIndex);
                generator.UpdateVerticeY(pixelIndex, noise);
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
        float offset = 5000f;
        float s = scale.Evaluate(1f);
        float noiseX = (offset + transform.position.x + x) / MeshGenerator.resolution * s;
        float noiseZ = (offset + transform.position.z + z) / MeshGenerator.resolution * s;
        float sample = Mathf.PerlinNoise(noiseX, noiseZ);

        float frequancy = sample * 8;
        return sample;

    }
}
