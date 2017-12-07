using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class HeightMapSettings : UpdatableData
{

    public NoiseSettings noiseSettings;

    public bool useFalloff;

    public float heightMultiplier;
    //Move this to Biome Settings. Also need to look into how I clamp this or adjust it
    [Tooltip("This is the maximum height that the Terrain can Generate. It Acts a a cap to generate Textures")]
    public float maximumMapHeight;
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