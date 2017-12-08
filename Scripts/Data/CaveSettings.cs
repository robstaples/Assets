using System.Collections;
using UnityEngine;

[CreateAssetMenu()]
public class CaveSettings : UpdatableData
	{
		[Tooltip("The Width of the cave (X-Axis)")]
    	public int width;
		[Tooltip("The Depth of the cave (Z-axis)")]
  		public int height;
		[Tooltip("The Height of the cave Walls (Y-axis)")]
		public int wallHeight;

		[Tooltip("Option to use a random seed instead a set seed")]
		public bool useRandomSeed;
		[Tooltip("The seed of the cave map. this can be anything")]
		public string seed;

		[Range (40,50)]
		[Tooltip("The Height of the cave Walls (Y-axis)")]
		public int randomFillPercent;

		[Header ("Advanced")]
		[Tooltip("The size of border around the edge of the cave")]
		public int borderSize = 1;
		[Tooltip("The width of the Passageways between Caverns")]
		public int passageWidth = 3;
		[Tooltip("The number of smoothing runs made on the cave walls")]
		public int smooth = 5;

		int[,] map;

			#if UNITY_EDITOR

				protected override void OnValidate()
				{
					base.OnValidate();
				}
			#endif
  }
