using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class HeightMapSettings : UpdatableData
{

    public NoiseSettings noiseSettings;

    public bool useFalloff;

    public float heightMultiplier;
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