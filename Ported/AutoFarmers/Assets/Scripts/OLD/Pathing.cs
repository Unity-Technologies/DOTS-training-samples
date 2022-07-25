using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathing {

	public delegate bool IsNavigableDelegate(int x,int y);
	public delegate bool CheckMatchDelegate(int x,int y);

	static int[,] visitedTiles;
	static List<int> activeTiles;
	static List<int> nextTiles;
	static List<int> outputTiles;

	static int[] dirsX = new int[] { 1,-1,0,0 };
	static int[] dirsY = new int[] { 0,0,1,-1 };

	static int mapWidth;
	static int mapHeight;

	public static RectInt fullMapZone;
	
	public static int Hash(int x,int y) {
		return y * mapWidth + x;
	}
	public static void Unhash(int hash,out int x,out int y) {
		y = hash / mapWidth;
		x = hash % mapHeight;
	}

	public static bool IsNavigableDefault(int x, int y) {
		return (Farm.tileRocks[x,y] == null);
	}
	public static bool IsNavigableAll(int x, int y) {
		return true;
	}

	public static bool IsRock(int x, int y) {
		return (Farm.tileRocks[x,y] != null);
	}
	public static bool IsStore(int x, int y) {
		return Farm.storeTiles[x,y];
	}
	public static bool IsTillable(int x, int y) {
		return (Farm.groundStates[x,y] == GroundState.Default);
	}
	public static bool IsReadyForPlant(int x,int y) {
		return (Farm.groundStates[x,y] == GroundState.Tilled);
	}

	public static void Init() {
		fullMapZone = new RectInt(0,0,Farm.instance.mapSize.x,Farm.instance.mapSize.y);
	}

	public static Rock FindNearbyRock(int x, int y, int range, Path outputPath) {
		int rockPosHash = SearchForOne(x,y,range,IsNavigableDefault,IsRock,fullMapZone);
		if (rockPosHash == -1) {
			return null;
		} else {
			int rockX, rockY;
			Unhash(rockPosHash,out rockX,out rockY);
			if (outputPath != null) {
				AssignLatestPath(outputPath,rockX,rockY);
			}
			return Farm.tileRocks[rockX,rockY];
		}
	}

	public static void WalkTo(int x, int y, int range, CheckMatchDelegate CheckMatch, Path outputPath) {
		int storePosHash = SearchForOne(x,y,range,IsNavigableDefault,CheckMatch,fullMapZone);
		if (storePosHash!=-1) {
			int storeX, storeY;
			Unhash(storePosHash,out storeX,out storeY);
			if (outputPath!=null) {
				AssignLatestPath(outputPath,storeX,storeY);
			}
		}
	}

	public static int SearchForOne(int startX,int startY, int range, IsNavigableDelegate IsNavigable, CheckMatchDelegate CheckMatch, RectInt requiredZone) {
		outputTiles = Search(startX,startY,range,IsNavigable,CheckMatch,requiredZone,1);
		if (outputTiles.Count==0) {
			return -1;
		} else {
			return outputTiles[0];
		}
	}

	public static List<int> Search(int startX,int startY,int range,IsNavigableDelegate IsNavigable,CheckMatchDelegate CheckMatch,RectInt requiredZone, int maxResultCount=0) {
		mapWidth = Farm.instance.mapSize.x;
		mapHeight = Farm.instance.mapSize.y;

		if (visitedTiles==null) {
			visitedTiles = new int[mapWidth,mapHeight];
			activeTiles = new List<int>();
			nextTiles = new List<int>();
			outputTiles = new List<int>();
		}

		for (int x=0;x<mapWidth;x++) {
			for (int y = 0; y < mapHeight; y++) {
				visitedTiles[x,y] = -1;
			}
		}
		outputTiles.Clear();
		visitedTiles[startX,startY] = 0;
		activeTiles.Clear();
		nextTiles.Clear();
		nextTiles.Add(Hash(startX,startY));

		int steps = 0;

		while (nextTiles.Count > 0 && (steps<range || range==0)) {
			List<int> temp = activeTiles;
			activeTiles = nextTiles;
			nextTiles = temp;
			nextTiles.Clear();

			steps++;

			for (int i=0;i<activeTiles.Count;i++) {
				int x, y;
				Unhash(activeTiles[i],out x,out y);

				for (int j=0;j<dirsX.Length;j++) {
					int x2 = x + dirsX[j];
					int y2 = y + dirsY[j];

					if (x2<0 || y2<0 || x2>=mapWidth || y2>=mapHeight) {
						continue;
					}

					if (visitedTiles[x2,y2]==-1 || visitedTiles[x2,y2]>steps) {

						int hash = Hash(x2,y2);
						if (IsNavigable(x2,y2)) {
							visitedTiles[x2,y2] = steps;
							nextTiles.Add(hash);
						}
						if (x2 >= requiredZone.xMin && x2 <= requiredZone.xMax) {
							if (y2 >= requiredZone.yMin && y2 <= requiredZone.yMax) {
								if (CheckMatch(x2,y2)) {
									outputTiles.Add(hash);
									if (maxResultCount != 0 && outputTiles.Count >= maxResultCount) {
										return outputTiles;
									}
								}
							}
						}
					}
				}
			}
		}

		return outputTiles;
	}

	public static void AssignLatestPath(Path target,int endX, int endY) {
		target.Clear();

		int x = endX;
		int y = endY;

		target.xPositions.Add(x);
		target.yPositions.Add(y);

		int dist = int.MaxValue;
		while (dist>0) {
			int minNeighborDist = int.MaxValue;
			int bestNewX = x;
			int bestNewY = y;
			for (int i=0;i<dirsX.Length;i++) {
				int x2 = x + dirsX[i];
				int y2 = y + dirsY[i];
				if (x2 < 0 || y2 < 0 || x2 >= mapWidth || y2 >= mapHeight) {
					continue;
				}

				int newDist = visitedTiles[x2,y2];
				if (newDist !=-1 && newDist < minNeighborDist) {
					minNeighborDist = newDist;
					bestNewX = x2;
					bestNewY = y2;
				}
			}
			x = bestNewX;
			y = bestNewY;
			dist = minNeighborDist;
			target.xPositions.Add(x);
			target.yPositions.Add(y);
		}
	}
}
