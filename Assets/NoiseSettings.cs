using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public bool enabled = false;
    public int interations = 1;
    public float strength = 1f;
    public float roughness = 1f;
    public float frequancy = 1f;
    public float persistance = 0.5f;
    public float minValue = 0f;
}
