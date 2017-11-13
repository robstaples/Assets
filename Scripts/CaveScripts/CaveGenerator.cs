using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveGenerator : MonoBehaviour {

	public int width;
	public int height;
	public int smooth;
	public int border;

	public string seed;
	public bool useRandomSeed;

	[Range (0,100)]
	public int randomFillPercent;

	int[,] map;

	void Start() {
		GenerateMap ();
	}
	void Update() {
		if (Input.GetMouseButtonDown (0)) {
			GenerateMap ();
		}
	}

	void GenerateMap() {
		map = new int[width, height];
		RandomFillMap ();

		for (int i = 0; i < smooth; i++) {
			SmoothMap ();
		}

		ProcessMap();

		int borderSize = border;
		int [,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2]

		for (int x = 0; x < borderedMap.getLength(0); x++) {
			for (int y = 0; y < borderedMap.getLength(1); y++) {
				if (x>= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize) {
					borderedMap[x,y] = map[x-borderSize,y-borderSize];
				}
				else {
					borderedMap[x,y] = 1;
				}
			}
		}

		CaveMesh meshGen = GetComponent<CaveMesh> ();
		meshGen.GenerateMesh (borderedMap, 1);
	}

void ProcessMap() {
	List<List<Coord>> wallRegions = GetRegions(1);

	//make public
	int wallThreasholdSize = 50;
	foreach (List<Coord> wallRegion in wallRegions) {
		if (wallRegion.Count < wallThreasholdSize) {
			foreach (Coord tile in wallRegion) {
				map[tile.tileX, tile.tileY] = 0;
			}
		}
	}

	List<List<Coord>> roomRegions = GetRegions(0);

	//make public
	int roomThreasholdSize = 50;
	foreach (List<Coord> roomRegion in roomRegions) {
		if (roomRegion.Count < roomThreasholdSize) {
			foreach (Coord tile in roomRegion) {
				map[tile.tileX, tile.tileY] = 1;
			}
		}
	}
}

List<List<Coord>> GetRegions(int tileType) {
	List<List<Coord>> regions = new List<List<Coord>>();
	int[,] mapFlags = new int[width,height];

	for (int x = 0; x < width; x++) {
		for (int y = 0; y < height; y++) {
			if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
				List<Coord> newRegion = GetRegionTiles(x,y);
				regions.Add(newRegion);
				foreach (Coord tile in newRegion) {
					mapFlags[tile.tileX, tile.tileY] = 1;
				}
			}
		}
	}
	return regions;
}

	List<Coord> GetRegionTiles(int startX, int startY) {
		List<Coord> tiles = new List<Coord>();
		int[,] mapFlags = new int[width,height];
		int tileType = map[startX,startY];

		Queue<Coord> queue = new Queue<Coord>();
		queue.Enqueue(new Coord[startX, startY]);
		mapFlags[startX, startY] = 1;

		while (queue.Count > 0) {
			Coord tile = queue.Dequeue();
			tiles.Add(tile);

			for (int x = tile.tileX -1; x <= tile.tileX +1; x++) {
				for (int y = tile.tileY -1; y <= tile.tileY +1; y++) {
					if (IsInMapRange(x,y) && (y == tile.tileY || x == tile.tileX)) {
						if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
							mapFlags[x,y] == 1;
							queue.Enqueue(new Coord(x,y));
						}
					}
			}
		}
		return tiles;
	}

	bool IsInMapRange(int x, int y) {
		return x >= 0 && x < width && y >= 0 && y < height;
	}

	void RandomFillMap(){
		if (useRandomSeed) {
			seed = Time.time.ToString ();
		}
		System.Random psuedoRandom = new System.Random (seed.GetHashCode ());

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (x == 0 || x == width -1 || y == 0 || y == height - 1)
				{
					map [x, y] = 1;
				}
				else {
					map [x, y] = (psuedoRandom.Next (0, 100) < randomFillPercent) ? 1 : 0;
				}
			}
		}
	}

	void SmoothMap() {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				int neighourWallTiles = GetSurroundingWallCount (x, y);

				if (neighourWallTiles > 4)
					map [x, y] = 1;
				else if (neighourWallTiles < 4)
					map [x, y] = 0;
			}
		}
	}

	int GetSurroundingWallCount(int gridX, int gridY) {
		int wallCount = 0;
		for (int neighbourX = gridX -1; neighbourX <= gridX + 1; neighbourX++){
			for (int neighbourY = gridY -1; neighbourY <= gridY + 1; neighbourY++){
				if (IsInMapRange(neighbourX, neighbourY)) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += map [neighbourX, neighbourY];
					}
				} else {
					wallCount++;
				}
			}
		}
		return wallCount;
	}

	struct Cood {
		public int tileX;
		public int tileY;

		public Coord(int x, int y) {
			tileX = X;
			tileY = Y;
		}
	}
}
