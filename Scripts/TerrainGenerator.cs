using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public int coliderLODIndex;
    public LODInfo[] detailLevels;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;
    public WaterSettings waterSettings;

    public Transform viewer;
    public Material mapMaterial;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    float meshWorldSize;
    int chunksVisibleInViewDist;

	GameObject waterObject;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

	Dictionary<Vector2, WaterChunk> waterChunkDictionary = new Dictionary<Vector2, WaterChunk>();
    List<WaterChunk> visibleWaterChunks = new List<WaterChunk>();

	Dictionary<Vector2, WaterChunk> underwaterChunkDictionary = new Dictionary<Vector2, WaterChunk>();
	List<WaterChunk> visibleUnderwaterChunks = new List<WaterChunk>();

    void Start()
    {

        textureSettings.ApplyToMaterial(mapMaterial);
        textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        float maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        meshWorldSize = meshSettings.meshWorldSize;
        chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDst / meshWorldSize);

        UpdateVisibleChunks();
    }
    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        //checking Collider
        if (viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }

    }

    void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoord = new HashSet<Vector2>();
		//HashSet<Vector2> alreadyUpdatedWaterCoord = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count -1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoord.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();

			//alreadyUpdatedWaterCoord.Add(visibleWaterChunks[i].coord);
			//visibleWaterChunks[i].UpdateWaterChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for (int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if (!alreadyUpdatedChunkCoord.Contains(viewedChunkCoord))
                {
                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
						waterChunkDictionary[viewedChunkCoord].UpdateWaterChunk();
                    }
                    else
                    {
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, coliderLODIndex, transform, viewer, mapMaterial);
                        terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                        newChunk.onVisibilityChanged += OnChunkVisibilityChanged;
                        newChunk.Load();


						//overwater Object
						waterObject = Instantiate(waterSettings.waterPrefab, transform); 
						WaterChunk newWater = new WaterChunk(waterObject, true, viewedChunkCoord, heightMapSettings, meshSettings, waterSettings,detailLevels, transform, viewer);
						waterChunkDictionary.Add(viewedChunkCoord, newWater);
                        newWater.onVisibilityChanged += OnWaterVisibilityChanged;
						newWater.Load();

						//underwater Object
						waterObject = Instantiate(waterSettings.waterPrefab, transform); 
						WaterChunk newUnderwater = new WaterChunk(waterObject, false, viewedChunkCoord, heightMapSettings, meshSettings, waterSettings,detailLevels, transform, viewer);
						underwaterChunkDictionary.Add(viewedChunkCoord, newUnderwater);
						//newUnderwater.onVisibilityChanged += OnWaterVisibilityChanged;
						newUnderwater.Load();                    
					}
                }
            }
        }
    }
    void OnChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
            visibleTerrainChunks.Add(chunk);
            
        else
            visibleTerrainChunks.Remove(chunk);
    }
    void OnWaterVisibilityChanged(WaterChunk chunk, bool isVisible)
    {
        if (isVisible)
            visibleWaterChunks.Add(chunk);
        else
            visibleWaterChunks.Remove(chunk);
    }
}
[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numSupportedLOD - 1)]
    public int lod;
    public float visibleDstThreshold;

    public float sqrVisibleDstThreshold
    {
        get
        {
            return visibleDstThreshold * visibleDstThreshold;
        }
    }
}
