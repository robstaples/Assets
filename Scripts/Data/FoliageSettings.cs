using System.Collections;
using UnityEngine;
using System.Linq;

[CreateAssetMenu()]
public class FoliageSettings : UpdatableData
{
    public Vector2 drawSize;

    //Add drawmode system, Noise or random
    public enum PlaceMode { Random, Noise };

    public float randomOffset;
    public NoiseSettings noiseSettings;
    public ObjectPreFab[] objects;
    
    [System.Serializable]
    public class ObjectPreFab
    {
        public GameObject preFab;
        public float sizeMultiplier = 1f;
        [Range(0,1)]
        public float height;
        [Range(0, 1)]
        public float placementRange;
        [Range(1, 10)]
        public int preFabDensity;
    }
}
