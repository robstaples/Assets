using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveMesh {
	public List<Vector3> vertices;
	public List<int> triangles;
	public Vector2[] uvs;
	int[,] map;
	float squareSize;

	Dictionary<int, List<Triangle>> triangleDictionary;
	List<List<int>> outlines;
	HashSet<int> checkedVertices;

	public SquareGrid squareGrid;
	public WallCaveMesh wallCaveMesh;
	public GroundCaveMesh groundCaveMesh;
    CaveSettings caveSettings;

    public CaveMesh(float _squareSize, CaveSettings _caveSettings) {

        squareSize = _squareSize;
        caveSettings = _caveSettings;

        map = new int[caveSettings.width, caveSettings.height];
        vertices = new List<Vector3>();
		triangles = new List<int>();
		checkedVertices = new HashSet<int>();
		outlines = new List<List<int>>();
        triangleDictionary = new Dictionary<int, List<Triangle>>();

        GenerateMap();

        AddVertices();
		AddUVs();

		wallCaveMesh = new WallCaveMesh(vertices, map, triangleDictionary, outlines, checkedVertices);
		groundCaveMesh = new GroundCaveMesh(map, squareSize);
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

    public void GenerateMap()
    {
        RandomFillMap();

        for (int i = 0; i < caveSettings.smooth; i++)
        {
            SmoothMap();
        }

        ProcessMap();

        int[,] borderedMap = new int[caveSettings.width + caveSettings.borderSize * 2, caveSettings.height + caveSettings.borderSize * 2];

        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if (x >= caveSettings.borderSize && x < caveSettings.width + caveSettings.borderSize && y >= caveSettings.borderSize && y < caveSettings.height + caveSettings.borderSize)
                {
                    borderedMap[x, y] = map[x - caveSettings.borderSize, y - caveSettings.borderSize];
                }
                else
                {
                    borderedMap[x, y] = 1;
                }
            }
        }
        map = borderedMap;
    }

    void ProcessMap()
    {
        List<List<Coord>> wallRegions = GetRegions(1);

        //make public
        int wallThreasholdSize = 50;
        foreach (List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThreasholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0);

        int roomThreasholdSize = 50;
        List<Room> survivingRooms = new List<Room>();

        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThreasholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    map[tile.tileX, tile.tileY] = 1;
                }
            }
            else
            {
                survivingRooms.Add(new Room(roomRegion, map));
            }
        }
        survivingRooms.Sort();


        survivingRooms[0].isMainRoom = true;
        survivingRooms[0].isAccessibleFromMainRoom = true;

        ConnectClosestRooms(survivingRooms);
    }

    void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
    {

        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in allRooms)
            {
                if (room.isAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach (Room roomB in roomListB)
            {
                if (roomA == roomB || roomA.IsConnected(roomB))
                {
                    continue;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }
            if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(allRooms, true);
        }
    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line)
        {
            DrawCircle(c, caveSettings.passageWidth);
        }
    }

    void DrawCircle(Coord c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int realX = c.tileX + x;
                    int realY = c.tileY + y;
                    if (IsInMapRange(realX, realY))
                    {
                        map[realX, realY] = 0;
                    }
                }
            }
        }
    }

    List<Coord> GetLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;

        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
                y += step;
            else
                x += step;

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                    x += gradientStep;
                else
                    y += gradientStep;

                gradientAccumulation -= longest;
            }
        }
        return line;
    }

    Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-caveSettings.width / 2 + .5f + tile.tileX, 2, -caveSettings.height / 2 + .5f + tile.tileY);
    }

    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[caveSettings.width, caveSettings.height];

        for (int x = 0; x < caveSettings.width; x++)
        {
            for (int y = 0; y < caveSettings.height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);
                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }
        return regions;
    }

    List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[caveSettings.width, caveSettings.height];
        int tileType = map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
                    {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < caveSettings.width && y >= 0 && y < caveSettings.height;
    }

    void RandomFillMap()
    {
        if (caveSettings.useRandomSeed)
        {
            caveSettings.seed = Time.time.ToString();
        }
        System.Random psuedoRandom = new System.Random(caveSettings.seed.GetHashCode());

        for (int x = 0; x < caveSettings.width; x++)
        {
            for (int y = 0; y < caveSettings.height; y++)
            {
                if (x == 0 || x == caveSettings.width - 1 || y == 0 || y == caveSettings.height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (psuedoRandom.Next(0, 100) < caveSettings.randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < caveSettings.width; x++)
        {
            for (int y = 0; y < caveSettings.height; y++)
            {
                int neighourWallTiles = GetSurroundingWallCount(x, y);

                if (neighourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (IsInMapRange(neighbourX, neighbourY))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    class Room : IComparable<Room>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public int roomSize;
        public bool isAccessibleFromMainRoom;
        public bool isMainRoom;

        public Room()
        {

        }

        public Room(List<Coord> roomTiles, int[,] map)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();

            edgeTiles = new List<Coord>();
            foreach (Coord tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if (x == tile.tileX || y == tile.tileY)
                        {
                            if (map[x, y] == 1)
                            {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        public void SetAccessibleFromMainRoom()
        {
            if (!isAccessibleFromMainRoom)
            {
                isAccessibleFromMainRoom = true;
                foreach (Room connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB)
        {
            if (roomA.isAccessibleFromMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }
            else if (roomB.isAccessibleFromMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }
        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }

        public int CompareTo(Room otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
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
	public List<Vector3> vertices;
	public List<int> triangles;
	public Vector2[] uvs;
	float wallHeight = 5;

	Dictionary<int, List<Triangle>> triangleDictionary;
	List<List<int>> outlines;
	HashSet<int> checkedVertices;

    public List<Vector3> caveVertices;

    public WallCaveMesh(List<Vector3> _caveVertices, int[,]_map,  Dictionary<int, List<Triangle>> _triangleDictionary, List<List<int>> _outlines, HashSet<int> _checkedVertices) {

		vertices = new List<Vector3>();
		triangles = new List<int>();
        caveVertices = _caveVertices;

        triangleDictionary = _triangleDictionary;
		outlines = _outlines;
		checkedVertices = _checkedVertices;

        CreateWallMesh(_map);

    }

	void CreateWallMesh(int[,] map) {

		CalculateMeshOutlines();

		List<Vector3> wallVertices = new List<Vector3>();
		List<int> wallTriangles = new List<int>();
		Vector2[] wallUvs;

		foreach (List<int> outline in outlines) {
			for (int i = 0; i < outline.Count -1; i++) {
				int startIndex = wallVertices.Count;

				wallVertices.Add(caveVertices[outline[i]]); //left
				wallVertices.Add(caveVertices[outline[i + 1]]); //right
				wallVertices.Add(caveVertices[outline[i]] - Vector3.up * wallHeight); //bottomleft
				wallVertices.Add(caveVertices[outline[i + 1]]  - Vector3.up * wallHeight); //bottomright

				wallTriangles.Add(startIndex + 0);
				wallTriangles.Add(startIndex + 2);
				wallTriangles.Add(startIndex + 3);

				wallTriangles.Add(startIndex + 3);
				wallTriangles.Add(startIndex + 1);
				wallTriangles.Add(startIndex + 0);
			}
		}

		int tileAmount = 10;
		wallUvs = new Vector2[wallVertices.Count];
		for (int i = 0; i < wallVertices.Count; i++) {
			float	percentX = Mathf.InverseLerp(-map.GetLength(1)/2, -map.GetLength(1)/2, wallVertices[i].x) * tileAmount;
			float	percentY = Mathf.InverseLerp(-map.GetLength(0)/2, -map.GetLength(0)/2, wallVertices[i].z) * tileAmount;
			wallUvs[i] = new Vector2(percentX, percentY);
		}

		vertices = wallVertices;
		triangles = wallTriangles;
        uvs = wallUvs;
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
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs;
	int width;
	int height;
	float squareSize;
	float wallHeight = 5;

	public GroundCaveMesh(int[,] _map, float _squareSize) {
		squareSize = _squareSize;
		width = _map.GetLength(0);
		height = _map.GetLength(1);

		GroundVertices();
		GroundTriangles();
		GroundUVS();
	}

	//Change to return a mesh
	void GroundVertices () {

		vertices = new Vector3 [(width + 1) * (height + 1)];
		for (int y = 0, i = 0; y <= height; y++) {
			for (int x = 0; x <= width; x++, i++) {
				//Changed Vector position
				vertices[i] = new Vector3 (-width/2 + x * squareSize + squareSize/2, -wallHeight/2, -height/2 + y * squareSize + squareSize/2);
			}
		}
	}

	void GroundTriangles () {
		//Change to triangles struct;
		triangles = new int[width * height * 6];
		for (int ti = 0, vi = 0, y =0; y < height; y++, vi ++) {
			for (int x = 0; x < width; x++, ti +=6, vi++) {
				triangles [ti] = vi;
				triangles [ti + 3] = triangles [ti + 2] = vi + 1;
				triangles [ti + 4] = triangles [ti + 1] = vi + width + 1;
				triangles [ti + 5] = vi + width + 2;
			}
		}
	}

	void GroundUVS () {
		int tileAmount = 10;
		//Create a method or struct to calculate UV's for all methods {Refactor}
		uvs = new Vector2[vertices.Length];
		for (int i = 0; i < vertices.Length; i++) {
			float	percentX = Mathf.InverseLerp(-width/2, width/2, vertices[i].x) * tileAmount;
			float	percentY = Mathf.InverseLerp(-width/2, width/2, vertices[i].z) * tileAmount;
			uvs[i] = new Vector2(percentX, percentY);
		}
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
