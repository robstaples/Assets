using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterChunk
{
    public Vector2 coord;
    public event System.Action<WaterChunk, bool> onVisibilityChanged;

    GameObject waterObject;

    Transform viewer;
	LODInfo[] detailLevels;

    Bounds bounds;
    Vector2 sampleCentre;
	float maxViewDst;

	Vector2 viewerPosition
	{
		get
		{
			return new Vector2(viewer.position.x, viewer.position.z);
		}
	}

	public WaterChunk(GameObject waterObject, bool isOverWater, Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, WaterSettings waterSettings, LODInfo[] detailLevels, Transform parent, Transform viewer)
    {
        this.coord = coord;
        this.viewer = viewer;
		this.waterObject = waterObject;
		this.detailLevels = detailLevels;

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(sampleCentre, Vector2.one * meshSettings.meshWorldSize);

        //+Custom+ Initializing the water
        if (waterSettings.hasWater)
        {
			if (isOverWater) {
				//water Create
				waterObject.name = "Water Chunk"; 
				waterObject.transform.position = new Vector3 (position.x, waterSettings.waterLevel * heightMapSettings.heightMultiplier, position.y);
			} else {
				//underwater Create
				waterObject.transform.Rotate (0, 180, 180);
				waterObject.name = "UnderWater Chunk";
				waterObject.transform.position = new Vector3 (position.x, (waterSettings.waterLevel * heightMapSettings.heightMultiplier) -0.34f, position.y);
			}

			waterObject.transform.parent = parent;
        }
		maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
    }
    public void Load()
    {
		UpdateWaterChunk ();
    }
	public void UpdateWaterChunk()
	{
		float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
		bool wasVisible = IsVisible();
		bool visible = viewerDistanceFromNearestEdge <= maxViewDst;

		//Checking if the Terrain Chunk is visible, and adding or removing it from the visibleTerrainChunks accordingly
		if (wasVisible != visible)
		{
			SetVisible(visible);
			if (onVisibilityChanged != null)
			{
				onVisibilityChanged(this, visible);
			}
		}
	}
    public void SetVisible(bool visible)
    {
        waterObject.SetActive(visible);
    }
    public bool IsVisible()
    {
        return waterObject.activeSelf;
    }

}
