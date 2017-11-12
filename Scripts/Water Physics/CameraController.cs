using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private bool defaultFog;
    private bool isUnderwater;
    private Color normalColor;
    private Color underwaterColor;
    private Material defaultSkybox;
	public WaterSettings waterSettings;
	public HeightMapSettings heightSettings;


    void Start () {
        //cameraWaterLevel = (mapGenerator.waterLevel * mapGenerator.meshHeightMultiplier) - (transform.localPosition.y / 2);
        normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        underwaterColor = new Color(0.22f, 0.45f, 0.67f, 0.5f);
        defaultFog = RenderSettings.fog;
        defaultSkybox = RenderSettings.skybox;
}
    void Update()
    {
		if (transform.position.y < (waterSettings.waterLevel * heightSettings.heightMultiplier))
            isUnderwater = true;
        else
            isUnderwater = false;

            if (isUnderwater)
                SetUnderwater();
            if (!isUnderwater)
                SetNormal();
    }
    void SetNormal()
    {
        RenderSettings.fog = defaultFog;
        RenderSettings.fogColor = normalColor;
        RenderSettings.fogDensity = 0.002f;
        RenderSettings.skybox = defaultSkybox;
    }
    void SetUnderwater()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = underwaterColor;
        RenderSettings.fogDensity = 0.07f;
        RenderSettings.skybox = null;
    }
}
