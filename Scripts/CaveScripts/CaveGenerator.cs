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
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
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

//	void OnDrawGizmos() {
//		if (map != null) {
//			for (int x = 0; x < width; x++) {
//				for (int y = 0; y < height; y++) {
//					Gizmos.color = (map [x, y] == 1) ? Color.black : Color.white;
//					Vector3 pos = new Vector3 (-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
//					Gizmos.DrawCube (pos, Vector3.one);
//				}
//			}
//		}
//	}
}
