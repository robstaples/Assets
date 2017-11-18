using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveMesh {
	List<Vector3> vertices;
	List<int> triangles;
	Vector2[] uvs;
	int[,] map;
	float squareSize;

	Dictionary<int, List<Triangle>> triangleDictionary;
	List<List<int>> outlines;
	HashSet<int> checkedVertices;

	public SquareGrid squareGrid;
	public WallCaveMesh wallCaveMesh;
	public GroundCaveMesh groundCaveMesh;


	public CaveMesh(int[,] _map, float _squareSize) {

		map = _map;
		squareSize = _squareSize;

		vertices = new List<Vector3>();
		triangles = new List<int>();
		checkedVertices = new HashSet<int>();
		outlines = new List<List<int>>();
		triangleDictionary = new Dictionary<int, List<Triangle>> ();

		AddVertices();
		AddUVs();

		//Create wall mesh
		//Create ground mesh
	}

	void AddVertices() {

		triangleDictionary.Clear();
		outlines.Clear();
		checkedVertices.Clear();

		squareGrid = new SquareGrid (map, squareSize);

		for (int x = 0; x < squareGrid.squares.GetLength (0); x++) {
			for (int y = 0; y < squareGrid.squares.GetLength (1); y++) {
				TriangulateSquare (squareGrid.squares [x, y]);
			}
		}
	}
	void AddUVs() {
		int tileAmount = 10;
		uvs = new Vector2[vertices.Count];
		for (int i = 0; i < vertices.Count; i++) {
			float	percentX = Mathf.InverseLerp(-map.GetLength(0)/2*squareSize, map.GetLength(0)/2*squareSize, vertices[i].x) * tileAmount;
			float	percentY = Mathf.InverseLerp(-map.GetLength(0)/2*squareSize, map.GetLength(0)/2*squareSize, vertices[i].z) * tileAmount;
			uvs[i] = new Vector2(percentX, percentY);
		}
	}
	void TriangulateSquare(Square square) {
		switch (square.configuration) {
		case 0:
			break;
			//1 point
		case 1:
			meshFromPoints (square.centreLeft, square.centreBottom, square.bottomLeft);
			break;
		case 2:
			meshFromPoints (square.bottomRight, square.centreBottom, square.centreRight);
			break;
		case 4:
			meshFromPoints (square.topRight, square.centreRight, square.centreTop);
			break;
		case 8:
			meshFromPoints (square.topLeft, square.centreTop, square.centreLeft);
			break;
			//2 points
		case 3:
			meshFromPoints (square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
			break;
		case 6:
			meshFromPoints (square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
			break;
		case 9:
			meshFromPoints (square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
			break;
		case 12:
			meshFromPoints (square.topLeft, square.topRight, square.centreRight, square.centreLeft);
			break;
		case 5:
			meshFromPoints (square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
			break;
		case 10:
			meshFromPoints (square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
			break;
			//3 points
		case 7:
			meshFromPoints (square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
			break;
		case 11:
			meshFromPoints (square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
			break;
		case 13:
			meshFromPoints (square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
			break;
		case 14:
			meshFromPoints (square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
			break;
			//4 point
		case 15:
			meshFromPoints (square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
			checkedVertices.Add(square.topLeft.vertexIndex);
			checkedVertices.Add(square.topRight.vertexIndex);
			checkedVertices.Add(square.bottomRight.vertexIndex);
			checkedVertices.Add(square.bottomLeft.vertexIndex);
			break;
		}
	}
	void meshFromPoints(params Node[] points) {
		AssignVertices (points);

		if (points.Length >=3)
			CreateTriangle(points[0], points[1], points[2]);
		if(points.Length >=4)
			CreateTriangle(points[0], points[2], points[3]);
		if(points.Length >=5)
			CreateTriangle(points[0], points[3], points[4]);
		if(points.Length >=6)
			CreateTriangle(points[0], points[4], points[5]);
	}

	void AssignVertices(Node[] points) {
		for (int i = 0; i < points.Length; i++) {
			if (points [i].vertexIndex == -1) {
				points [i].vertexIndex = vertices.Count;
				vertices.Add (points [i].position);
			}
		}
	}

	void CreateTriangle(Node a, Node b, Node c) {
		triangles.Add (a.vertexIndex);
		triangles.Add (b.vertexIndex);
		triangles.Add (c.vertexIndex);

		Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
		AddTriangleToDictionary(triangle.vertexIndexA, triangle);
		AddTriangleToDictionary(triangle.vertexIndexB, triangle);
		AddTriangleToDictionary(triangle.vertexIndexC, triangle);
	}


	void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle){
		if (triangleDictionary.ContainsKey (vertexIndexKey)){
			triangleDictionary [vertexIndexKey].Add (triangle);
		}
		else {
			List<Triangle> triangleList = new List<Triangle>();
			triangleList.Add(triangle);
			triangleDictionary.Add(vertexIndexKey, triangleList);
		}
	}
}
public class SquareGrid {
	public Square[,] squares;

	public SquareGrid(int[,] map, float squareSize){
		int nodeCountX = map.GetLength(0);
		int nodeCountY = map.GetLength(1);
		float mapWidth = nodeCountX * squareSize;
		float mapHeight = nodeCountY * squareSize;

		ControlNode[,] controlNodes = new ControlNode[nodeCountX,nodeCountY];

		for (int x = 0; x <nodeCountX; x ++) {
			for (int y = 0; y <nodeCountY; y ++) {
				Vector3 pos = new Vector3(-mapWidth/2 + x * squareSize + squareSize/2, 0, -mapHeight/2 + y * squareSize + squareSize/2);

				controlNodes[x,y] = new ControlNode(pos, map[x,y] == 1, squareSize);
			}
		}

		squares = new Square[nodeCountX -1, nodeCountY -1];
		for (int x = 0; x <nodeCountX-1; x ++) {
			for (int y = 0; y <nodeCountY-1; y ++) {
				squares[x,y] = new Square(controlNodes[x,y+1], controlNodes[x+1,y+1],controlNodes[x+1,y],controlNodes[x,y]);
			}
		}
	}
}

public class Square{

	public ControlNode topLeft, topRight, bottomRight, bottomLeft;
	public Node centreTop, centreRight, centreBottom, centreLeft;
	public int configuration;


	public Square (ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)
	{
		topLeft = _topLeft;
		topRight = _topRight;
		bottomRight = _bottomRight;
		bottomLeft = _bottomLeft;

		centreTop = topLeft.right;
		centreRight = bottomRight.above;
		centreBottom = bottomLeft.right;
		centreLeft = bottomLeft.above;

		if (topLeft.active) {
			configuration += 8;
		}
		if (topRight.active) {
			configuration += 4;
		}
		if (bottomRight.active) {
			configuration += 2;
		}
		if (bottomLeft.active) {
			configuration += 1;
		}
	}
}

public class Node{
	public Vector3 position;
	public int vertexIndex = -1;

	public Node(Vector3 _pos){
		position = _pos;
	}

}

public class ControlNode : Node {

	public bool active;
	public Node above, right;

	public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
		active = _active;
		above = new Node(position + Vector3.forward * squareSize/2F);
		right = new Node(position + Vector3.right * squareSize/2F);

	}
}
public class WallCaveMesh {
	List<Vector3> vertices;
	List<int> triangles;
	List<Vector2> uvs;
	float wallHeight = 5;
	int[,] map;
	float squareSize;

	Dictionary<int, List<Triangle>> triangleDictionary;
	List<List<int>> outlines;
	HashSet<int> checkedVertices;

	public WallCaveMesh(int[,] _map, float _squareSize, Dictionary<int, List<Triangle>> _triangleDictionary, List<List<int>> _outlines, HashSet<int> _checkedVertices) {

		vertices = new List<Vector3>();
		triangles = new List<int>();
		uvs = new List<Vector2>();

		map = _map;
		squareSize = _squareSize;

		triangleDictionary = _triangleDictionary;
		outlines = _outlines;
		checkedVertices = _checkedVertices;
	}

	void CreateWallMesh(int[,] map, Mesh caveMesh) {

		CalculateMeshOutlines();

		List<Vector3> wallVertices = new List<Vector3>();
		List<int> wallTriangles = new List<int>();
		Mesh wallMesh = new Mesh();
		Vector2[] wallUvs;

		foreach (List<int> outline in outlines) {
			for (int i = 0; i < outline.Count -1; i++) {
				int startIndex = wallVertices.Count;

				wallVertices.Add(vertices[outline[i]]); //left
				wallVertices.Add(vertices[outline[i + 1]]); //right
				wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); //bottomleft
				wallVertices.Add(vertices[outline[i + 1]]  - Vector3.up * wallHeight); //bottomright

				wallTriangles.Add(startIndex + 0);
				wallTriangles.Add(startIndex + 2);
				wallTriangles.Add(startIndex + 3);

				wallTriangles.Add(startIndex + 3);
				wallTriangles.Add(startIndex + 1);
				wallTriangles.Add(startIndex + 0);
			}
		}

		int tileAmount = 10;
		//Create a method to calculate UV's for all methods {Refactor}
		wallUvs = new Vector2[wallVertices.Count];
		for (int i = 0; i < wallVertices.Count; i++) {
			float	percentX = Mathf.InverseLerp(-map.GetLength(1)/2, -map.GetLength(1)/2, wallVertices[i].x) * tileAmount;
			float	percentY = Mathf.InverseLerp(-map.GetLength(0)/2, -map.GetLength(0)/2, wallVertices[i].z) * tileAmount;
			wallUvs[i] = new Vector2(percentX, percentY);
		}

		wallMesh.vertices = wallVertices.ToArray();
		wallMesh.triangles = wallTriangles.ToArray();
		wallMesh.uv = wallUvs;
	}
	void CalculateMeshOutlines() {
		for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++) {
			if (!checkedVertices.Contains(vertexIndex)) {
				int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
				if (newOutlineVertex != -1) {
					checkedVertices.Add(vertexIndex);

					List<int> newOutline = new List<int>();
					newOutline.Add(vertexIndex);
					outlines.Add(newOutline);
					FollowOutline(newOutlineVertex, outlines.Count -1);
					outlines[outlines.Count-1].Add(vertexIndex);
				}
			}
		}
	}
	void FollowOutline(int vertexIndex, int outlineIndex) {
		outlines[outlineIndex].Add (vertexIndex);
		checkedVertices.Add (vertexIndex);
		int nextVertexIndex = GetConnectedOutlineVertex (vertexIndex);

		if (nextVertexIndex != -1) {
			FollowOutline(nextVertexIndex, outlineIndex);
		}
	}
	int GetConnectedOutlineVertex(int vertexIndex) {
		List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

		for (int i=0; i < trianglesContainingVertex.Count; i++) {
			Triangle triangle = trianglesContainingVertex[i];

			for (int j=0; j < 3; j++){
				int vertexB = triangle [j];
				if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB)) {
					if (isOutlineEdge(vertexIndex, vertexB)) {
						return vertexB;
					}
				}
			}
		}

		return -1;
	}

	bool isOutlineEdge (int vertexA, int vertexB) {
		List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];
		int sharedTriangleCount = 0;
		for (int i=0; i <trianglesContainingVertexA.Count; i++) {
			if (trianglesContainingVertexA[i].Contains(vertexB)) {
				sharedTriangleCount++;
				if (sharedTriangleCount > 1) {
					break;
				}
			}
		}
		return sharedTriangleCount == 1;
	}
}

public class GroundCaveMesh {
	Vector3[] vertices;
	int[] triangles;
	Vector2[] uvs;

	//Change to return a mesh
	Mesh CreateGroundMesh (int width, int height, int squareSize) {
		Mesh groundMesh = new Mesh ();
		//Change to list to bring into alignment with other meshes{Refactor}
		Vector3[] groundVertices;
		int[] groundTriangles;
		Vector2[] groundUvs;

		groundVertices = new Vector3 [(width + 1) * (height + 1)];
		for (int y = 0, i = 0; y <= height; y++) {
			for (int x = 0; x <= width; x++, i++) {
				//Changed Vector position
				groundVertices[i] = new Vector3 (-width/2 + x * squareSize + squareSize/2, -wallHeight/2, -height/2 + y * squareSize + squareSize/2);
			}
		}
		groundMesh.vertices = groundVertices;

		groundTriangles = new int[width * height * 6];
		for (int ti = 0, vi = 0, y =0; y < height; y++, vi ++) {
			for (int x = 0; x < width; x++, ti +=6, vi++) {
				groundTriangles [ti] = vi;
				groundTriangles [ti + 3] = groundTriangles [ti + 2] = vi + 1;
				groundTriangles [ti + 4] = groundTriangles [ti + 1] = vi + width + 1;
				groundTriangles [ti + 5] = vi + width + 2;
			}
		}
		groundMesh.triangles = groundTriangles;
		//This should be a variable in the Editor {Refactor}
		int tileAmount = 10;
		//Create a method to calculate UV's for all methods {Refactor}
		groundUvs = new Vector2[groundVertices.Length];
		for (int i = 0; i < groundVertices.Length; i++) {
			float	percentX = Mathf.InverseLerp(-width/2, width/2, groundVertices[i].x) * tileAmount;
			float	percentY = Mathf.InverseLerp(-width/2, width/2, groundVertices[i].z) * tileAmount;
			groundUvs[i] = new Vector2(percentX, percentY);
		}
		groundMesh.uv = groundUvs;

		return groundMesh;
	}
}
public struct Triangle {
	public int vertexIndexA;
	public int vertexIndexB;
	public int vertexIndexC;
	int[] vertices;

	public Triangle (int a, int b, int c) {
		vertexIndexA = a;
		vertexIndexB = b;
		vertexIndexC = c;

		vertices = new int[3];
		vertices[0] = a;
		vertices[1] = b;
		vertices[2] = c;
	}

	public int this[int i] {
		get {
			return vertices[i];
		}
	}
	public bool Contains(int vertexIndex){
		return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
	}
}

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