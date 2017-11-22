using System;
using System.Collections.Generic;
using UnityEngine;
using Sebastian.Geomerry;


public class ShapeCreator : MonoBehaviour
{
	public MeshFilter MeshFilter;

	[HideInInspector]
	public List<Shape> shapes = new List<Shape>();
	
	[HideInInspector]
	public bool showShapesList;

    public float handleRadius = .5f;
	public void UpdateMeshDisplay()
	{
		CompositeShape compShape = new CompositeShape(shapes);
		MeshFilter.mesh = compShape.GetMesh();
	}
}
