using System.Collections;
using UnityEngine;

public class CavePreview : MonoBehaviour {
    public bool autoUpdate;

    //Cave MeshData
    public CaveMesh caveMesh;
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

    public Material caveMaterial;
    public Material wallMaterial;
    public Material groundMaterial;

    public void DrawMapInEditor(int[,] map, float squareSize) {

//		if (drawMode == DrawMode.twoD) {
//		caveMesh = CaveMesh.GenerateCaveMesh(map, squareSize, true);
//	public void Generate2DColliders() {
//
//		EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D>();
//		for (int i = 0; i < currentColliders.Length; i++) {
//			Destroy(currentColliders[i]);
//		}
//
//		CalculateMeshOutlines();
//
//		foreach (List<int> outline in outlines) {
//			EdgeCollider2D edgeCollider2D = gameObject.AddComponent<EdgeCollider2D>();
//			Vector2[] edgePoints = new Vector2[outline.Count];
//
//			for (int i = 0; i < outline.Count; i++) {
//				edgePoints[i] = new Vector2(vertices[outline[i]].x, vertices[outline[i]].z);
//			}
//			edgeCollider2D.points = edgePoints;
//		}
//	}

//		}

		//if (drawMode == DrawMode.threeD) {
    CaveGenerator meshGen = GetComponent<CaveGenerator> ();
    meshGen.GenerateMap();

		caveMesh = new CaveMesh(map, squareSize, false);

		DrawCaveMesh(caveMesh);
		//}

    }
	//This will draw all the required meshes for the cave
    public void DrawCaveMesh(CaveMesh caveMesh)
	{
    Mesh cMesh = new Mesh();
    cMesh.vertices = caveMesh.vertices.ToArray();
		cMesh.triangles = caveMesh.triangles.ToArray();
    cMesh.uv = caveMesh.uvs;
    cMesh.RecalculateNormals();
		caveMeshFilter.sharedMesh = cMesh;
		caveMeshRenderer.gameObject.SetActive (true);

    Mesh wMesh = new Mesh();
    wMesh.vertices = caveMesh.wallCaveMesh.vertices.ToArray();
    wMesh.triangles = caveMesh.wallCaveMesh.triangles.ToArray();
    wMesh.uv = caveMesh.wallCaveMesh.uvs;
    wMesh.RecalculateNormals();
    wallMeshFilter.sharedMesh = wMesh;
		wallMeshRenderer.gameObject.SetActive (true);

    Mesh gMesh = new Mesh();
    gMesh.vertices = caveMesh.groundCaveMesh.vertices.ToArray();
    gMesh.triangles = caveMesh.groundCaveMesh.triangles.ToArray();
    gMesh.uv = caveMesh.groundCaveMesh.uvs;
    gMesh.RecalculateNormals();
    groundMeshFilter.sharedMesh = gMesh;
		groundMeshRenderer.gameObject.SetActive (true);
	}
}
