using System.Collections;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public bool autoUpdate;

    //Cave MeshData
    public MeshFilter caveMeshFilter;
    public MeshRenderer caveMeshRenderer;

    //Add support for flatshading. {Scope increase}
    public enum DrawMode{2D, 3D};

    //Wall MeshData
    public MeshFilter wallMeshFilter;
    public MeshRenderer wallMeshRenderer;

    //Ground MeshData
    public MeshFilter groundMeshFilter;
    public MeshRenderer groundMeshRenderer;

    //public CaveSettings caveSettings;
    //public TextureData textureData;

    //public Material caveMaterial;

    public void DrawMapInEditor(int[,] map, float squareSize) {

      if (DrawMode == 2D) {
        CaveMesh.GenerateCaveMesh(map, squareSize);
        CaveMesh.Generate2DColliders();
      }

      if (DrawMode == 3D) {
        Mesh caveMesh = CaveMesh.GenerateCaveMesh(map, squareSize);

        DrawCaveMesh(caveMesh);
        DrawWallMesh(CaveMesh.CreateWallMesh(caveMesh));
        DrawGroundMesh(CaveMesh.CreateGoundMesh (map.GetLength (0), map.GetLength (1)));
      }

    }
    public void DrawCaveMesh(Mesh meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshFilter.gameObject.SetActive(true);
    }
    public void DrawWallMesh(Mesh meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshFilter.gameObject.SetActive(true);
    }
    public void DrawGroundMesh(Mesh meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshFilter.gameObject.SetActive(true);
    }
}
