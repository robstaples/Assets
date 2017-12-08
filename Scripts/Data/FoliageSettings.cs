using System.Collections;
using UnityEngine;
using System.Linq;

[CreateAssetMenu()]
public class FoliageSettings : UpdatableData
{
    //Remove and replace with Chunk size. or change the implementation
    public Vector2 drawSize;

    [Tooltip("Adds a random offset to the placement. This is to offset the grid nature ")]
    public float randomOffset;
    public NoiseSettings noiseSettings;
   
    [Tooltip("How many objects are in the array")]
    public ObjectPreFab[] objects;
    
    [System.Serializable]
    public class ObjectPreFab
    {
        [Tooltip("The Object Prefab")]
        public GameObject preFab;

        [Tooltip("Multiplies the size of the object.")]
        public float sizeMultiplier = 1f;
        
        [Range(0,1)]
        [Tooltip("Height on the noisemap that the object starts at")]
        public float height;
        
        [Range(0, 1)]
        [Tooltip("Height that the objects ends at.")]
        public float placementRange;
        
        [Range(1, 10)]
        [Tooltip("The density that the prefab appears on the grid.")]
        public int preFabDensity;
    }
}
