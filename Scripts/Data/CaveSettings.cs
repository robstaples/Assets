using System.Collections;
using UnityEngine;

[CreateAssetMenu()]
public class CaveSettings : UpdatableData
	{
    public int width;
  	public int height;

  	public string seed;
  	public bool useRandomSeed;

  	[Range (40,50)]
  	public int randomFillPercent;

  	[Header ("Advanced")]
  	public int borderSize = 1;
  	public int passageWidth = 3;
  	public int smooth = 5;

    int[,] map;

		#if UNITY_EDITOR

		    protected override void OnValidate()
		    {
		        base.OnValidate();
		    }
		#endif
  }
