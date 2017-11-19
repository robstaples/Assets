using System.Collections;
using UnityEngine;

public class CavePreview : MonoBehaviour {
    public bool autoUpdate;

    public CaveMesh caveMesh;
    public MeshFilter caveMeshFilter;
    public MeshRenderer caveMeshRenderer;

    public MeshFilter wallMeshFilter;
    public MeshRenderer wallMeshRenderer;

    public MeshFilter groundMeshFilter;
    public MeshRenderer groundMeshRenderer;

    public Material caveMaterial;
    public Material wallMaterial;
    public Material groundMaterial;

    public CaveSettings caveSettings;

	float squareSize = 1;

    public void DrawMapInEditor() {

		caveMesh = new CaveMesh(squareSize, caveSettings);

		DrawCaveMesh(caveMesh);

    }
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
        gMesh.vertices = caveMesh.groundCaveMesh.vertices;
        gMesh.triangles = caveMesh.groundCaveMesh.triangles;
        gMesh.uv = caveMesh.groundCaveMesh.uvs;
        gMesh.RecalculateNormals();
        groundMeshFilter.sharedMesh = gMesh;
        groundMeshRenderer.gameObject.SetActive (true);
	}
}
