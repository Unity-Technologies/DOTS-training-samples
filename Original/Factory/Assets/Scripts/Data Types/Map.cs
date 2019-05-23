using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map {
	public MapTile[,] tiles;
	public int[,] occupants;
	public int[,] occupantTickers;
	public int occupantTicker = 0;
	public int maxCost = 100;
	public int width;
	public int height;

	public Map(int myWidth, int myHeight,string mapString) {
		int i, j;

		width = myWidth;
		height = myHeight;

		tiles = new MapTile[myWidth,myHeight];
		occupants = new int[myWidth,myHeight];
		occupantTickers = new int[myWidth,myHeight];
	}

	public bool IsInsideMap(Vector2Int tile) {
		if (tile.x<0 || tile.x>=width) {
			return false;
		}
		if (tile.y<0 || tile.y>=height) {
			return false;
		}

		return true;
	}
	public bool IsWall(Vector2Int tile) {
		if (tile.x<0 || tile.x>=width || tile.y<0 || tile.y>=height) {
			return true;
		}

		if (tiles[tile.x,tile.y].moveCost == maxCost) {
			return true;
		} else {
			return false;
		}
	}
}
