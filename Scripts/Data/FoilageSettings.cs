using System.Collections;
using UnityEngine;

[CreateAssetMenu()]
public class FoilageSettings : UpdatableData
	{

		public NoiseSettings noiseSettings;

		public float heightMultiplier;
		public AnimationCurve heightCurve;
		
		public GameObject bigTree;
		public GameObject smallTree;
		public GameObject bigRock;
		public GameObject smallRock;

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
		public HeightMapSettings ConvertToHeightMap()
		{
		return new HeightMapSettings (heightCurve, heightMultiplier, false, noiseSettings);
		}
	}
