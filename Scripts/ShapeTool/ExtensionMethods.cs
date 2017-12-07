using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {
    public static Vector2 ToXZ (this Vector3 V3)
    {
        return new Vector2(V3.x, V3.y);
    }
    public static bool isBetween(float number, float min, float max)
    {
        return number >= min && number <= max;
    }
    public static int InverseDensity(int i, int rangeTop)
    {
        i = rangeTop + 1 - i;
        return i;
    }
}
