using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;

public static class Pathing 
{

	public delegate bool IsNavigableDelegate(int x,int y, Settings settings, DynamicBuffer<TileState> tiles);
	public delegate bool CheckMatchDelegate(int x,int y, Settings settings, DynamicBuffer<TileState> tiles);

	/*static int[,] visitedTiles;
	static List<int> activeTiles;
	static List<int> nextTiles;
	static List<int> outputTiles;

	

	static int mapWidth;
	static int mapHeight;

	public static RectInt fullMapZone;
	*/

	public static bool IsNavigableAll(int x, int y, Settings settings, DynamicBuffer<TileState> tiles) {
		return true;
	}

	public static bool IsNavigableDefault(int x, int y, Settings settings, DynamicBuffer<TileState> tiles)
	{
		return (tiles[x + y * settings.GridSize.x].Value != TileStates.Rock);
	}

	public static bool IsRock(int x, int y, Settings settings, DynamicBuffer<TileState> tiles)
	{
		return (tiles[x + y * settings.GridSize.x].Value == TileStates.Rock);
	}

	public static bool IsStore(int x, int y, Settings settings, DynamicBuffer<TileState> tiles)
	{
		return (tiles[x + y * settings.GridSize.x].Value == TileStates.Store);
	}

	public static bool IsTillable(int x, int y, Settings settings, DynamicBuffer<TileState> tiles)
	{
		return (tiles[x + y * settings.GridSize.x].Value == TileStates.Empty);
	}

	public static bool IsReadyForPlant(int x,int y, Settings settings, DynamicBuffer<TileState> tiles) {
		return (tiles[x+ y * settings.GridSize.x].Value == TileStates.Tilled);
	}

	public static TileState FindNearbyRock(DynamicBuffer<PathNode> pathNodes, DynamicBuffer<TileState> tiles, int2 position, int range, Settings settings) 
	{
		NativeArray<int> visitedTiles;
		int rockPosHash = SearchForOne(position, range, settings, tiles, IsNavigableDefault, IsRock, new RectInt(0, 0, settings.GridSize.x, settings.GridSize.y), out visitedTiles);
		if (rockPosHash == -1) {
			return new TileState { Value = TileStates.Empty};
		} else {
			int rockX = rockPosHash % settings.GridSize.x;
			int rockY = rockPosHash / settings.GridSize.y; 
			//Unhash(rockPosHash,out rockX,out rockY);
			AssignLatestPath(pathNodes,rockX,rockY, settings.GridSize, visitedTiles);
			visitedTiles.Dispose();
			return tiles[rockPosHash];
		}
	}

	public static void WalkTo(DynamicBuffer<PathNode> pathNodes, DynamicBuffer<TileState> tileStates, int2 initPosition, int range, Settings settings, CheckMatchDelegate CheckMatch)
	{
		NativeArray<int> visitedTiles;
		int storePosHash = SearchForOne(initPosition, range, settings, tileStates, IsNavigableDefault,CheckMatch,new RectInt(0,0, settings.GridSize.x, settings.GridSize.y), out visitedTiles);
		if (storePosHash!=-1) {
			int storeX = storePosHash % settings.GridSize.x;
			int storeY = storePosHash / settings.GridSize.y;
			//Unhash(storePosHash,out storeX,out storeY);
			AssignLatestPath(pathNodes, storeX, storeY, settings.GridSize, visitedTiles);
			visitedTiles.Dispose();
		}
	}

	public static int SearchForOne(int2 initPosition, int range, Settings settings, DynamicBuffer<TileState> tileStates, IsNavigableDelegate IsNavigable, CheckMatchDelegate CheckMatch, RectInt requiredZone, out NativeArray<int> visitedTiles) {
		var outputTiles = Search(initPosition, range, settings, tileStates, IsNavigable, CheckMatch,requiredZone, out visitedTiles, 1);
		if (outputTiles.Length==0) {
			outputTiles.Dispose();
			return -1;
		} else {
			var result = outputTiles[0];
			outputTiles.Dispose();
			return result;
		}
	}

	public static NativeList<int> Search(int2 initPosition,int range, Settings settings, DynamicBuffer<TileState> tileStates, IsNavigableDelegate IsNavigable,CheckMatchDelegate CheckMatch,RectInt requiredZone, out NativeArray<int> visitedTiles, int maxResultCount=0) {
		
		var farmWidth = settings.GridSize.x;
		var farmHeight = settings.GridSize.y;
        visitedTiles = new NativeArray<int>(farmWidth * farmHeight, Allocator.Temp);
		var activeTiles = new NativeList<int>(Allocator.Temp);
		var nextTiles = new NativeList<int>(Allocator.Temp);
		var outputTiles = new NativeList<int>(Allocator.Temp);

		for (int x=0;x<settings.GridSize.x;x++) {
			for (int y = 0; y < settings.GridSize.y; y++) {
				var linerIndex = x + y * settings.GridSize.x;
				visitedTiles[linerIndex] = -1;
			}
		}
		var initialPositionIndex = initPosition.x + initPosition.y * settings.GridSize.x;
		visitedTiles[initialPositionIndex] = 0;
		
		nextTiles.Add(initialPositionIndex);

		int steps = 0;

		NativeArray<int2> dirs = new NativeArray<int2>(4, Allocator.Temp);
		dirs[0] = new int2(1, 0);
		dirs[1] = new int2(-1, 0);
		dirs[2] = new int2(0, 1);
		dirs[3] = new int2(0, -1);

		while (nextTiles.Length > 0 && (steps < range || range==0)) {
			
			var temp = activeTiles;
			activeTiles = nextTiles;
			nextTiles = temp;
			nextTiles.Clear();

			steps++;

			for (int i=0;i<activeTiles.Length;i++) {

				int x = activeTiles[i] % settings.GridSize.x;
				int y = activeTiles[i] / settings.GridSize.y;

				//Unhash(activeTiles[i],out x,out y);

				for (int j = 0; j < dirs.Length; j++) {
					int x2 = x + dirs[j].x;
					int y2 = y + dirs[j].y;

					if (x2 < 0 || y2 < 0 || x2 >= settings.GridSize.x || y2 >= settings.GridSize.y) {
						continue;
					}
					var nextTileIndex = x2 + y2 * settings.GridSize.x;
					if (visitedTiles[nextTileIndex] ==-1 || visitedTiles[nextTileIndex] >steps) {

						if (IsNavigable(x2,y2, settings, tileStates)) {
							visitedTiles[nextTileIndex] = steps;
							nextTiles.Add(nextTileIndex);
						}
						if (x2 >= requiredZone.xMin && x2 <= requiredZone.xMax) {
							if (y2 >= requiredZone.yMin && y2 <= requiredZone.yMax) {
								if (CheckMatch(x2,y2, settings, tileStates)) {
									outputTiles.Add(nextTileIndex);
									if (maxResultCount != 0 && outputTiles.Length >= maxResultCount) {
										activeTiles.Dispose();
										nextTiles.Dispose();
										dirs.Dispose();
										return outputTiles;
									}
								}
							}
						}
					}
				}
			}
		}

		activeTiles.Dispose();
		nextTiles.Dispose();
		dirs.Dispose();
		return outputTiles;
	}

	public static void AssignLatestPath(DynamicBuffer<PathNode> pathNodes, int endX, int endY, int2 gridSize, NativeArray<int> visitedTiles) 
	{
		NativeArray<int2> dirs = new NativeArray<int2>(4, Allocator.Temp);
		dirs[0] = new int2(1, 0);
		dirs[1] = new int2(-1, 0);
		dirs[2] = new int2(0, 1);
		dirs[3] = new int2(0, -1);

		int x = endX;
		int y = endY;
		var newPath = new PathNode { Value = new int2(x, y) };
		pathNodes.Add(newPath);

		int dist = int.MaxValue;
		while (dist>0) {
			int minNeighborDist = int.MaxValue;
			int bestNewX = x;
			int bestNewY = y;

			for (int i=0;i< dirs.Length;i++) {
				
				int x2 = x + dirs[i].x;
				int y2 = y + dirs[i].y;
				if (x2 < 0 || y2 < 0 || x2 >= gridSize.x || y2 >= gridSize.y) {
					continue;
				}

				int newDist = visitedTiles[x2 + y2 * gridSize.x];
				if (newDist !=-1 && newDist < minNeighborDist) {
					minNeighborDist = newDist;
					bestNewX = x2;
					bestNewY = y2;
				}
			}
			x = bestNewX;
			y = bestNewY;
			dist = minNeighborDist;
			
			newPath = new PathNode { Value = new int2(x, y) };
			pathNodes.Add(newPath);
		}
		dirs.Dispose();
	}
}
