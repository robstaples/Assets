using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceFoilage : MonoBehaviour {

	public enum DrawMode { NoiseMap, Texture };
	public DrawMode drawMode;

	public Renderer noiseMap;
	public Renderer texturePlane;

	FoilageSettings foilageSettings;

	// Use this for initialization
	void Start () {
		
	}
	public void PlaceFoilageInEditor()
	{
		HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(245, 245, foilageSettings.ConvertToHeightMap(), Vector2.zero);

		if (drawMode == DrawMode.NoiseMap) {
			DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
		}
		else if (drawMode == DrawMode.Texture) {
			DrawPlane ();
		}
	}
	public void DrawTexture(Texture2D texture)
	{
		noiseMap.sharedMaterial.mainTexture = texture;
		noiseMap.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

		noiseMap.gameObject.SetActive(true);
		texturePlane.gameObject.SetActive(false);
	}
	public void DrawPlane()
	{
		noiseMap.gameObject.SetActive(false);
		texturePlane.gameObject.SetActive(true);
	}
	void OnValuesUpdated()
	{
		if (!Application.isPlaying)
		{
			PlaceFoilageInEditor();
		}
	}
}
