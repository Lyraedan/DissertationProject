﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum NoiseType
    {
        Perlin, Perlin_Abs, Simplex
    }

    public NoiseType noiseType = NoiseType.Perlin;
    public bool enabled = false;
    public uint interations = 1;
    public float strength = 1f;
    public float roughness = 1f;
    public float frequancy = 1f;
    public float persistance = 0.5f;
    public float minValue = 0f;
}
