using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {
    public static Vector2 ToXZ (this Vector3 V3)
    {
        return new Vector2(V3.x, V3.y);
    }
}
