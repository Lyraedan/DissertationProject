using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils : MonoBehaviour
{
    public static float DistanceFrom(Vector3 src, Vector3 dest)
    {
        return Mathf.Sqrt(Mathf.Pow(dest.x - src.x, 2) +
                          Mathf.Pow(dest.y - src.y, 2) +
                          Mathf.Pow(dest.z - src.z, 2));
    }
}
