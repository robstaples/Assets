using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliagePreview : MonoBehaviour
{
    [HideInInspector]
    public bool autoUpdate;

    public Renderer noiseRender;
    public Renderer grassRender;
    public GameObject foliage;

    public FoliageSettings foliageSettings;
    public bool isRand = false;

    public void DrawInEditor()
    {
        if (!isRand)
        {
            noiseRender.gameObject.SetActive(true);
            grassRender.gameObject.SetActive(false);

            //Get dimensions of plane
            float[,] values = Noise.GenerateNoiseMap((int)Mathf.Round(foliageSettings.drawSize.x), (int)Mathf.Round(foliageSettings.drawSize.y), foliageSettings.noiseSettings, Vector2.zero);
            DrawTexture(TextureGenerator.TextureFromNoise(values, values.GetLength(0), values.GetLength(1)));

            SpawnObjectNoise(values);
        }
        else
        {
            noiseRender.gameObject.SetActive(false);
            grassRender.gameObject.SetActive(true);

            for (int i = 0; i < foliageSettings.objects.Length; i++)
            {
                SpawnObjectRand(foliageSettings.objects[i].preFab, foliageSettings.objects[i].sizeMultiplier);
            }
        }
    }

    public void DrawTexture(Texture2D texture)
    {
        noiseRender.sharedMaterial.mainTexture = texture;
        noiseRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

        noiseRender.gameObject.SetActive(true);
    }

    public void DestroyAllObjects()
    {
        for (int i = foliage.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(foliage.transform.GetChild(i).gameObject);
        }
    }

    void SpawnObjectNoise(float[,] values)
    {
        for (int i = 0; i < foliageSettings.objects.Length; i++)
        {
            for (int x = 0; x < values.GetLength(0); x = x + ExtensionMethods.InverseDensity(foliageSettings.objects[i].preFabDensity, 10))
            {
                for (int y = 0; y < values.GetLength(1); y = y + ExtensionMethods.InverseDensity(foliageSettings.objects[i].preFabDensity, 10))
                {
                float minRange = values[x, y] - foliageSettings.objects[i].placementRange;
                float maxRange = values[x, y] + foliageSettings.objects[i].placementRange;
                    if (ExtensionMethods.isBetween(foliageSettings.objects[i].height, minRange, maxRange)) {
                        Vector3 pos = new Vector3(Random.Range(x - foliageSettings.randomOffset, x + foliageSettings.randomOffset), 0, Random.Range(y - foliageSettings.randomOffset, y + foliageSettings.randomOffset));
                        GameObject newObject = Instantiate(foliageSettings.objects[i].preFab, pos, Quaternion.identity);
                        newObject.transform.parent = foliage.transform;
                        newObject.transform.localScale = new Vector3 (foliageSettings.objects[i].sizeMultiplier, foliageSettings.objects[i].sizeMultiplier, foliageSettings.objects[i].sizeMultiplier);
                        newObject.transform.Rotate(0, Random.Range(0, 360), 0);

                    }
                }
            }
        }
    }

    void SpawnObjectRand(GameObject gb, float size)
    {
        
        //Will need to refactored before It can be integrated with ojbects
        Vector3 pos = transform.localPosition + new Vector3(Random.Range(-foliageSettings.drawSize.x / 2, foliageSettings.drawSize.x / 2), 0, Random.Range(-foliageSettings.drawSize.y / 2, foliageSettings.drawSize.y / 2));

        GameObject newObject = Instantiate(gb, pos, Quaternion.identity);
        newObject.transform.parent = transform;
        newObject.transform.localScale = new Vector3(size, size, size);
    }
}
