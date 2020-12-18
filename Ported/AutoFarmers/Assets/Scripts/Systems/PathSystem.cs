using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PathSystem : SystemBase
{
    public static NativeArray<ETileState> defaultNavigation;
	public static NativeArray<ETileState> isRock;
	public static NativeArray<ETileState> isTillable;
	public static NativeArray<ETileState> isReadyToPlant;
	public static NativeArray<ETileState> isStore;
	public static NativeArray<ETileState> isPlant;

    protected override void OnCreate()
	{

		defaultNavigation = new NativeArray<ETileState>(5, Allocator.Persistent);
		defaultNavigation[0] = ETileState.Empty;
		defaultNavigation[1] = ETileState.Grown;
		defaultNavigation[2] = ETileState.Seeded;
		defaultNavigation[3] = ETileState.Store;
		defaultNavigation[4] = ETileState.Tilled;

		isRock = new NativeArray<ETileState>(1, Allocator.Persistent);
        isRock[0] = ETileState.Rock;

        isTillable = new NativeArray<ETileState>(1, Allocator.Persistent);
        isTillable[0] = ETileState.Empty;

        isStore = new NativeArray<ETileState>(1, Allocator.Persistent);
        isStore[0] = ETileState.Store;

        isReadyToPlant = new NativeArray<ETileState>(1, Allocator.Persistent);
        isReadyToPlant[0] = ETileState.Tilled;

        isPlant = new NativeArray<ETileState>(1, Allocator.Persistent);
        isPlant[0] = ETileState.Seeded;
	}

	protected override void OnUpdate() { }

	protected override void OnDestroy()
	{
		defaultNavigation.Dispose();
		isRock.Dispose();
		isTillable.Dispose();

		isReadyToPlant.Dispose();
		isStore.Dispose();
		isPlant.Dispose();
	}

	public static int Hash(int x, int y, RectInt fullMapZone)
	{
		return y * fullMapZone.width + x;
	}
	public static void Unhash(int hash, RectInt fullMapZone, out int x, out int y)
	{
		y = hash / fullMapZone.width;
		x = hash % fullMapZone.width;
	}

    public static int FindNearbyRock(int x, int y, int range, DynamicBuffer<TileState> tiles, NativeArray<ETileState> navigable, NativeArray<ETileState> match, DynamicBuffer<PathNode> outputPath, RectInt fullMapZone)
    {
        NativeArray<int> visitedTiles;
        int rockPosHash = SearchForOne(x, y, range, tiles, navigable, match, fullMapZone, fullMapZone, out visitedTiles);
        if (rockPosHash == -1)
		{
			visitedTiles.Dispose();
			return -1; //ETileState.Empty;
        }
        else
        {
            int rockX, rockY;
            Unhash(rockPosHash, fullMapZone, out rockX, out rockY);
            if (outputPath.IsCreated)
            {
                AssignLatestPath(outputPath, rockX, rockY, fullMapZone, visitedTiles);
            }
			visitedTiles.Dispose();

			return Hash(rockX, rockY, fullMapZone); //tiles[Hash(rockX, rockY)].Value;
        }
    }

    public static int WalkTo(int x, int y, int range, DynamicBuffer<TileState> tiles, NativeArray<ETileState> navigable, NativeArray<ETileState> match, DynamicBuffer<PathNode> outputPath, RectInt fullMapZone)
	{
		NativeArray<int> visitedTiles;

		int storePosHash = SearchForOne(x, y, range, tiles, navigable, match, fullMapZone, fullMapZone, out visitedTiles);
		if (storePosHash != -1)
		{
			int storeX, storeY;
			Unhash(storePosHash, fullMapZone, out storeX, out storeY);
			if (outputPath.IsCreated)
			{
				AssignLatestPath(outputPath, storeX, storeY, fullMapZone, visitedTiles);
			}
			visitedTiles.Dispose();
			return storePosHash;
		}
		visitedTiles.Dispose();
		return -1;
	}

	public static int SearchForOne(int startX, int startY, int range, DynamicBuffer<TileState> tiles, NativeArray<ETileState> navigable, NativeArray<ETileState> match, RectInt requiredZone, RectInt fullMapZone, out NativeArray<int> visitedtiles)
	{
		var outputTiles = Search(startX, startY, range, tiles, navigable, match, requiredZone, fullMapZone, out visitedtiles, 1);
		if (outputTiles.Length == 0)
		{
			outputTiles.Dispose();
			return -1;
		}
		else
		{
			var value = outputTiles[0];
			outputTiles.Dispose();
			return value;
		}
	}

	public static NativeList<int> Search(int startX, int startY, int range, DynamicBuffer<TileState> tiles, NativeArray<ETileState> navigable, NativeArray<ETileState> match, RectInt requiredZone, RectInt fullMapZone, out NativeArray<int> visitedTiles, int maxResultCount = 0)
	{
		var dirs = new NativeArray<int2>(4, Allocator.Temp);
		dirs[0] = new int2(1, 0);
		dirs[1] = new int2(-1, 0);
		dirs[2] = new int2(0, 1);
		dirs[3] = new int2(0, -1);

		NativeList<int> activeTiles = new NativeList<int>(Allocator.Temp);
		NativeList<int> nextTiles = new NativeList<int>(Allocator.Temp);
		NativeList<int> outputTiles = new NativeList<int>(Allocator.Temp);
		visitedTiles = new NativeArray<int>(fullMapZone.width * fullMapZone.height, Allocator.Temp);

		for (int x = 0; x < fullMapZone.width; x++)
		{
			for (int y = 0; y < fullMapZone.height; y++)
			{
				visitedTiles[Hash(x, y, fullMapZone)] = -1;
			}
		}

		visitedTiles[Hash(startX, startY, fullMapZone)] = 0;
		nextTiles.Add(Hash(startX, startY, fullMapZone));

		int steps = 0;

		while (nextTiles.Length > 0 && (steps < range || range == 0))
		{
			NativeList<int> temp = activeTiles;
			activeTiles = nextTiles;
			nextTiles = temp;
			nextTiles.Clear();

			steps++; ;

			for (int i = 0; i < activeTiles.Length; i++)
			{
				int x, y;
				Unhash(activeTiles[i], fullMapZone, out x, out y);

				for (int j = 0; j < dirs.Length; j++)
				{
					int x2 = x + dirs[j].x;
					int y2 = y + dirs[j].y;

					if (x2 < 0 || y2 < 0 || x2 >= fullMapZone.width || y2 >= fullMapZone.height)
					{
						continue;
					}
					var hash2 = Hash(x2, y2, fullMapZone);
					if (visitedTiles[hash2] == -1 || visitedTiles[hash2] > steps)
					{
						for (int k = 0; k < navigable.Length; k++)
						{
							if (navigable[k] == tiles[hash2].Value)
							{
								visitedTiles[hash2] = steps;
								nextTiles.Add(hash2);
							}
						}
						if (x2 >= requiredZone.xMin && x2 <= requiredZone.xMax)
						{
							if (y2 >= requiredZone.yMin && y2 <= requiredZone.yMax)
							{

								for (int k = 0; k < match.Length; k++)
								{
									if (match[k] == tiles[hash2].Value)
									{
										outputTiles.Add(hash2);
										if (maxResultCount != 0 && outputTiles.Length >= maxResultCount)
										{
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
		}

		activeTiles.Dispose();
		nextTiles.Dispose();
		dirs.Dispose();
		return outputTiles;
	}

	public static void AssignLatestPath(DynamicBuffer<PathNode> target, int endX, int endY, RectInt fullMapZone, NativeArray<int> visitedTiles)
	{
		var dirs = new NativeArray<int2>(4, Allocator.Temp);
		dirs[0] = new int2(1, 0);
		dirs[1] = new int2(-1, 0);
		dirs[2] = new int2(0, 1);
		dirs[3] = new int2(0, -1);

		target.Clear();

		int x = endX;
		int y = endY;

		target.Add(new PathNode { Value = new int2(x, y) });

		int dist = int.MaxValue;
		while (dist > 0)
		{
			int minNeighborDist = int.MaxValue;
			int bestNewX = x;
			int bestNewY = y;
			for (int i = 0; i < dirs.Length; i++)
			{
				int x2 = x + dirs[i].x;
				int y2 = y + dirs[i].y;
				if (x2 < 0 || y2 < 0 || x2 >= fullMapZone.width || y2 >= fullMapZone.height)
				{
					continue;
				}

				int newDist = visitedTiles[Hash(x2, y2, fullMapZone)];
				if (newDist != -1 && newDist < minNeighborDist)
				{
					minNeighborDist = newDist;
					bestNewX = x2;
					bestNewY = y2;
				}
			}
			x = bestNewX;
			y = bestNewY;
			dist = minNeighborDist;
			target.Add(new PathNode { Value = new int2(x, y) });
		}
		dirs.Dispose();
	}
}
