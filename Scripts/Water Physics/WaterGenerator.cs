using System.Collections;
using UnityEngine;

public static class WaterGenerator{

    public static GameObject GenerateWater(GameObject waterObject, WaterSettings waterSettings, MeshSettings meshSettings, HeightMapSettings heightMapSettings)
    {
        waterObject.transform.position = new Vector3(0, waterSettings.waterLevel * heightMapSettings.heightMultiplier, 0);
        waterObject.transform.localScale = new Vector3(meshSettings.meshWorldSize / 1.2f, 0, meshSettings.meshWorldSize / 1.2f);

        return waterObject;
    }
}
