using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class HeightMapSettings : UpdatableData
{

    public NoiseSettings noiseSettings;

    [Tooltip("The option to turn the noise map into a falloff map.")]
    public bool useFalloff;

    [Tooltip("The multiplyer that controls the multiplier that is assigned to the noise map. The multiplier against the Maximum height to generate Textures")]
    public float heightMultiplier;
    //placeholder for elevation.
    //[Tooltip("The elevation is the minimum height of chunk size")]
    //public float Elevation;
    //Move this to Biome Settings. Also need to look into how I clamp this or adjust it
    [Tooltip("This is the maximum height that the Terrain can Generate. It Acts a a cap to generate Textures")]
    public float maximumMapHeight;
    [Tooltip("The curve that controls the height progression against the noisemap")]
    public AnimationCurve heightCurve;

	public HeightMapSettings(AnimationCurve heightCurve, float heightMultiplier, bool useFalloff, NoiseSettings noiseSettings)
	{
		this.heightCurve = heightCurve;
		this.heightMultiplier = heightMultiplier;
		this.useFalloff = useFalloff;
		this.noiseSettings = noiseSettings;
	}

    public float minHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(1);
        }
    }

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        noiseSettings.ValidateValues();
        base.OnValidate();
    }
#endif

}