using System.Collections;
using UnityEngine;

public class CavePreview : MonoBehaviour
{
    public bool autoUpdate;

    //Cave MeshData
    public MeshFilter caveMeshFilter;
    public MeshRenderer caveMeshRenderer;

    //Add support for flatshading. {Scope increase}
    //public enum DrawMode{ twoD, threeD };
	//public DrawMode drawMode;

    //Wall MeshData
    public MeshFilter wallMeshFilter;
    public MeshRenderer wallMeshRenderer;

    //Ground MeshData
    public MeshFilter groundMeshFilter;
    public MeshRenderer groundMeshRenderer;

    //public CaveSettings caveSettings;
    //public TextureData textureData;

    //public Material caveMaterial;
    //public Material wallMaterial;
    //public Material groundMaterial;

    public void DrawMapInEditor(int[,] map, float squareSize) {
		Mesh caveMesh = new Mesh ();

//		if (drawMode == DrawMode.twoD) {
//		caveMesh = CaveMesh.GenerateCaveMesh(map, squareSize, true);
//		caveMesh = CaveMesh.Generate2DColliders();
//		}

		//if (drawMode == DrawMode.threeD) {
		caveMesh = new CaveMesh(map, squareSize, false);

		DrawCaveMesh(caveMesh);
		//}

    }
	//This will draw all the required meshes for the cave
    public void DrawCaveMesh(Mesh meshData)
	{
		caveMeshFilter.sharedMesh = meshData;
		caveMeshRenderer.gameObject.SetActive (true);
	}
}
