using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FarmInitializeSystem))]
public class PathMovement : SystemBase
{
    NativeArray<int> visitedTiles;
    NativeList<int> activeTiles;
    NativeList<int> nextTiles;
    NativeList<int> outputTiles;

    NativeArray<ETileState> defaultNativation;
	NativeArray<ETileState> rock;

	NativeArray<int2> dirs;

    int mapWidth;
    int mapHeight;

	RectInt fullMapZone;

    protected override void OnCreate()
    {
        activeTiles = new NativeList<int>(Allocator.Persistent);
        nextTiles = new NativeList<int>(Allocator.Persistent);
        outputTiles = new NativeList<int>(Allocator.Persistent);

        dirs = new NativeArray<int2>(4, Allocator.Persistent);
        dirs[0] = new int2(1, 0);
        dirs[1] = new int2(-1, 0);
        dirs[2] = new int2(0, 1);
        dirs[3] = new int2(0, -1);

        defaultNativation = new NativeArray<ETileState>(5, Allocator.Persistent);
		defaultNativation[0] = ETileState.Empty;
		defaultNativation[1] = ETileState.Grown;
		defaultNativation[2] = ETileState.Seeded;
		defaultNativation[3] = ETileState.Store;
		defaultNativation[4] = ETileState.Tilled;

        rock = new NativeArray<ETileState>(1, Allocator.Persistent);
        rock[0] = ETileState.Rock;
    }


    protected override void OnUpdate()
    {
        var settings = GetSingleton<CommonSettings>();

		if (mapWidth != settings.GridSize.x || mapHeight != settings.GridSize.y)
		{
			mapWidth = settings.GridSize.x;
			mapHeight = settings.GridSize.y;
			fullMapZone = new RectInt(0, 0, mapWidth, mapHeight);
			if (visitedTiles.IsCreated)
				visitedTiles.Dispose();
			visitedTiles = new NativeArray<int>(mapWidth * mapHeight, Allocator.Persistent);
		}
        var data = GetSingletonEntity<CommonData>();
        
        var pathBuffers = GetBufferFromEntity<PathNode>();
        var tileBuffer = GetBufferFromEntity<TileState>()[data];

        Entities
            .ForEach((Entity entity, ref Velocity velocity, in Translation translation) =>
            {
                var pathNodes = pathBuffers[entity];

                var farmerPosition = new int2((int)math.floor(translation.Value.x), 
                                           (int)math.floor(translation.Value.z));

                if (pathNodes.Length > 0)
                {
                    for (int i = 0; i < pathNodes.Length - 1; i++)
                    {
                        Debug.DrawLine(new Vector3(pathNodes[i].Value.x + .5f, .5f, pathNodes[i].Value.y + .5f), new Vector3(pathNodes[i + 1].Value.x + .5f, .5f, pathNodes[i + 1].Value.y + .5f), Color.red);
                    }

                    var targetPosition = pathNodes[pathNodes.Length - 1].Value;

                    if (farmerPosition.x == targetPosition.x && farmerPosition.y == targetPosition.y)
                    {
                        pathNodes.RemoveAt(pathNodes.Length - 1);
                    }
                    else
                    {
                        bool isBlocked = false;
                        if (targetPosition.x < 0 || targetPosition.y < 0 || targetPosition.x >= settings.GridSize.x || targetPosition.y >= settings.GridSize.y)
                        {
                            isBlocked |= true;
                        }
                        var nextTileState = tileBuffer[targetPosition.x + targetPosition.y * settings.GridSize.x].Value;
                        if (nextTileState == ETileState.Rock)
                        {
                            isBlocked |= true;
                        }

                        if (!isBlocked)
                        {
                            float offset = .5f;
                            if (nextTileState == ETileState.Grown)
                            {
                                offset = .01f;
                            }
                            velocity.Value = new float3(targetPosition.x + offset, 0, targetPosition.y + offset);
                        }
                    }
                }
                else 
                {
                    velocity.Value = 0;
                }
            }).Run();
    }

    protected override void OnDestroy()
	{
		visitedTiles.Dispose();
		activeTiles.Dispose();
		nextTiles.Dispose();
		outputTiles.Dispose();

		defaultNativation.Dispose();
		rock.Dispose();

		dirs.Dispose();
	}

    public int Hash(int x, int y)
    {
        return y * mapWidth + x;
    }
    public void Unhash(int hash, out int x, out int y)
    {
        y = hash / mapWidth;
        x = hash % mapHeight;
    }

	public ETileState FindNearbyRock(int x, int y, int range, DynamicBuffer<TileState> tiles, DynamicBuffer<PathNode> outputPath)
	{
		int rockPosHash = SearchForOne(x, y, range, tiles, defaultNativation, rock, fullMapZone);
		if (rockPosHash == -1)
		{
			return ETileState.Empty;
		}
		else
		{
			int rockX, rockY;
			Unhash(rockPosHash, out rockX, out rockY);
			if (outputPath.IsCreated)
			{
				AssignLatestPath(outputPath, rockX, rockY);
			}
			return tiles[Hash(rockX, rockY)].Value;
		}
	}

	public void WalkTo(int x, int y, int range, DynamicBuffer<TileState> tiles, NativeArray<ETileState> match, DynamicBuffer<PathNode> outputPath)
	{
		int storePosHash = SearchForOne(x, y, range, tiles, defaultNativation, match, fullMapZone);
		if (storePosHash != -1)
		{
			int storeX, storeY;
			Unhash(storePosHash, out storeX, out storeY);
			if (outputPath.IsCreated)
			{
				AssignLatestPath(outputPath, storeX, storeY);
			}
		}
	}

	public int SearchForOne(int startX, int startY, int range, DynamicBuffer<TileState> tiles, NativeArray<ETileState> navigable, NativeArray<ETileState> match, RectInt requiredZone)
	{
		outputTiles = Search(startX, startY, range, tiles, navigable, match, requiredZone, 1);
		if (outputTiles.Length == 0)
		{
			return -1;
		}
		else
		{
			return outputTiles[0];
		}
	}

	public NativeList<int> Search(int startX, int startY, int range, DynamicBuffer<TileState> tiles, NativeArray<ETileState> navigable, NativeArray<ETileState> match, RectInt requiredZone, int maxResultCount = 0)
	{
		for (int x = 0; x < mapWidth; x++)
		{
			for (int y = 0; y < mapHeight; y++)
			{
				visitedTiles[Hash(x, y)] = -1;
			}
		}

		outputTiles.Clear();
		visitedTiles[Hash(startX, startY)] = 0;
		activeTiles.Clear();
		nextTiles.Clear();
		nextTiles.Add(Hash(startX, startY));

		int steps = 0;

		while (nextTiles.Length > 0 && (steps < range || range == 0))
		{
			NativeList<int> temp = activeTiles;
			activeTiles = nextTiles;
			nextTiles = temp;
			nextTiles.Clear();

			steps++;;

			for (int i = 0; i < activeTiles.Length; i++)
			{
				int x, y;
				Unhash(activeTiles[i], out x, out y);

				for (int j = 0; j < dirs.Length; j++)
				{
					int x2 = x + dirs[j].x;
					int y2 = y + dirs[j].y;

					if (x2 < 0 || y2 < 0 || x2 >= mapWidth || y2 >= mapHeight)
					{
						continue;
					}
					var hash2 = Hash(x2, y2);
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

		return outputTiles;
	}

	public void AssignLatestPath(DynamicBuffer<PathNode> target, int endX, int endY)
	{
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
				if (x2 < 0 || y2 < 0 || x2 >= mapWidth || y2 >= mapHeight)
				{
					continue;
				}

				int newDist = visitedTiles[Hash(x2, y2)];
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
	}
}