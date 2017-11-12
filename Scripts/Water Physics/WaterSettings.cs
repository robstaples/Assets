using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class WaterSettings : UpdatableData
{

    public bool hasWater;
    [Range(0, 1)]
    public float waterLevel;

    public GameObject waterPrefab;

    ////Underwater Create
    //underwaterObject = Instantiate(waterPrefab, parent);
    //underwaterObject.name = "Underwater Chunk";
    //            underwaterObject.transform.localScale = new Vector3(MapGenerator.mapChunkSize* scale, 0, MapGenerator.mapChunkSize* scale);
    //underwaterObject.transform.position = new Vector3(positionV3.x* scale, (mapGenerator.waterLevel* mapGenerator.meshHeightMultiplier) - 0.2f, positionV3.z* scale);
    //underwaterObject.transform.Rotate(0, 180, 180);
    //        }

    //    //+custom+ Setting the chunks to visible
    //if (mapGenerator.hasWater)
    //{
    //    waterObject.SetActive(visible);
    //    underwaterObject.SetActive(visible); 
    //}
}
