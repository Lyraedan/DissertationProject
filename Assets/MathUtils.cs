using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MathUtils : MonoBehaviour
{
    public static float DistanceFrom(Vector3 src, Vector3 dest)
    {
        return Mathf.Sqrt(Mathf.Pow(dest.x - src.x, 2) +
                          Mathf.Pow(dest.y - src.y, 2) +
                          Mathf.Pow(dest.z - src.z, 2));
    }

    public static long nanoTime()
    {
        long nano = 10000L * Stopwatch.GetTimestamp();
        nano /= TimeSpan.TicksPerMillisecond;
        nano *= 100L;
        return nano;
    }
}
