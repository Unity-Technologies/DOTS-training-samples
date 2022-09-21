using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TileCreationSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PrefabConfig>();
		state.RequireForUpdate<MazeConfig>();
	}

    public void OnDestroy(ref SystemState state)
    {
    }

	public int Get1DIndex(int xIndex, int yIndex)
    {
		MazeConfig mazeConfig = SystemAPI.GetSingleton<MazeConfig>();
		return xIndex + yIndex * mazeConfig.Width;
    }

	public void CreateGround(ref SystemState state)
    {
		PrefabConfig prefabConfig = SystemAPI.GetSingleton<PrefabConfig>();
		MazeConfig mazeConfig = SystemAPI.GetSingleton<MazeConfig>();

		Entity groundEntity = state.EntityManager.Instantiate(prefabConfig.TilePrefab);
		TransformAspect transformAspect = SystemAPI.GetAspectRW<TransformAspect>(groundEntity);
		transformAspect.Position += new float3(mazeConfig.Width * 0.5f - 0.5f, 0.0f, mazeConfig.Height * 0.5f - 0.5f);

		PostTransformMatrix postTransformMatrix = SystemAPI.GetComponent<PostTransformMatrix>(groundEntity);
		postTransformMatrix.Value = float4x4.Scale(mazeConfig.Width, postTransformMatrix.Value.c1.y, mazeConfig.Height);
		SystemAPI.SetComponent<PostTransformMatrix>(groundEntity, postTransformMatrix);
	}

    public void OnUpdate(ref SystemState state)
    {
		CreateGround(ref state);

        PrefabConfig prefabConfig = SystemAPI.GetSingleton<PrefabConfig>();
		MazeConfig mazeConfig = SystemAPI.GetSingleton<MazeConfig>();

		Entity entity = state.EntityManager.CreateEntity();
        DynamicBuffer<TileBufferElement> tiles = state.EntityManager.AddBuffer<TileBufferElement>(entity);
        tiles.Capacity = mazeConfig.Width * mazeConfig.Height;

        TileBufferElement tileBufferElement = new TileBufferElement
        {
            UpWall = true,
            DownWall = true,
            RightWall = true,
            LeftWall = true
        };

        for (int i = 0; i < mazeConfig.Width * mazeConfig.Height; ++i)
        {
			tiles.Add(tileBufferElement);
        }

		Stack<Vector2Int> stack = new Stack<Vector2Int>();
		Vector2Int current = mazeConfig.GetRandomTilePosition();
		TileBufferElement tempTile = tiles[Get1DIndex(current.x, current.y)];
		tempTile.TempVisited = true;
		tiles[Get1DIndex(current.x, current.y)] = tempTile;
		int numVisited = 1;

		while (numVisited < tiles.Length)
		{
			// choose random adjacent unvisited tile
			List<Vector2Int> unvisitedNeighbors = new List<Vector2Int>();
			if (current.x > 0 && !tiles[Get1DIndex(current.x - 1, current.y)].TempVisited)
				unvisitedNeighbors.Add(new Vector2Int(current.x - 1, current.y));
			if (current.y > 0 && !tiles[Get1DIndex(current.x, current.y - 1)].TempVisited)
				unvisitedNeighbors.Add(new Vector2Int(current.x, current.y - 1));
			if (current.x < mazeConfig.Width - 1 && !tiles[Get1DIndex(current.x + 1, current.y)].TempVisited)
				unvisitedNeighbors.Add(new Vector2Int(current.x + 1, current.y));
			if (current.y < mazeConfig.Height - 1 && !tiles[Get1DIndex(current.x, current.y + 1)].TempVisited)
				unvisitedNeighbors.Add(new Vector2Int(current.x, current.y + 1));

			if (unvisitedNeighbors.Count > 0)
			{
				// visit neighbor
				Vector2Int next = unvisitedNeighbors[UnityEngine.Random.Range(0, unvisitedNeighbors.Count)];
				stack.Push(current);
				// remove wall between tiles
				TileBufferElement currentTile = tiles[Get1DIndex(current.x, current.y)];
				TileBufferElement nextTile = tiles[Get1DIndex(next.x, next.y)];
				if (next.x > current.x)
				{
					currentTile.RightWall = false;
					nextTile.LeftWall = false;
				}
				else if (next.y > current.y)
				{
					currentTile.UpWall = false;
					nextTile.DownWall = false;
				}
				else if (next.x < current.x)
				{
					currentTile.LeftWall = false;
					nextTile.RightWall = false;
				}
				else
				{
					currentTile.DownWall = false;
					nextTile.UpWall = false;
				}
				nextTile.TempVisited = true;
				numVisited++;
				tiles[Get1DIndex(current.x, current.y)] = currentTile;
				tiles[Get1DIndex(next.x, next.y)] = nextTile;
				current = next;
			}
			else
			{
				// backtrack if no unvisited neighboring tiles
				if (stack.Count > 0)
				{
					current = stack.Pop();
				}
			}
		}

		TileBufferElement emptyTile = new TileBufferElement();
		if (mazeConfig.OpenStrips > 0 || mazeConfig.MazeStrips > 0)
		{
			for (int x = mazeConfig.OpenStrips + mazeConfig.MazeStrips; x < mazeConfig.Width - mazeConfig.OpenStrips; x += mazeConfig.OpenStrips + mazeConfig.MazeStrips)
			{
				for (int i = 0; i < mazeConfig.OpenStrips; ++i)
				{
					for (int y = 0; y < mazeConfig.Height; ++y)
					{
						tiles[Get1DIndex(x + i, y)] = emptyTile;
					}
				}
			}
		}

		// Clear out left border
		for (int x = 0; x < mazeConfig.OpenStrips; ++x)
        {
			for (int y = 0; y < mazeConfig.Height; ++y)
            {
				tiles[Get1DIndex(x, y)] = emptyTile;
			}
		}

		// Clear out right border
		for (int x = mazeConfig.Width - 1; x > mazeConfig.Width - mazeConfig.OpenStrips; --x)
		{
			for (int y = 0; y < mazeConfig.Height; ++y)
			{
				tiles[Get1DIndex(x, y)] = emptyTile;
			}
		}

		// Clear out bottom border
		for (int y = 0; y < mazeConfig.OpenStrips; ++y)
		{
			for (int x = 0; x < mazeConfig.Width; ++x)
			{
				tiles[Get1DIndex(x, y)] = emptyTile;
			}
		}

		// Clear out top border
		for (int y = mazeConfig.Height - 1; y > mazeConfig.Height - mazeConfig.OpenStrips; --y)
		{
			for (int x = 0; x < mazeConfig.Width; ++x)
			{
				tiles[Get1DIndex(x, y)] = emptyTile;
			}
		}



		NativeArray<MovingWall> spawnedMovingWalls = CollectionHelper.CreateNativeArray<MovingWall>(mazeConfig.MovingWallsToSpawn, Allocator.Temp);

		float3 positionOffset = float3.zero;
		if (mazeConfig.MovingWallSize % 2 == 0)
		{
			positionOffset -= new float3(0.5f, 0.0f, 0.0f);
		}

		// Spawn moving walls
		for (int i = 0; i < mazeConfig.MovingWallsToSpawn; ++i)
		{
			int tilesToMove = UnityEngine.Random.Range(mazeConfig.MovingWallMinTilesToMove, mazeConfig.MovingWallMaxTilesToMove);
			Entity wallEntity = state.EntityManager.Instantiate(prefabConfig.MovingWallPrefab);
			TransformAspect transformAspect = SystemAPI.GetAspectRW<TransformAspect>(wallEntity);

			Vector2Int randomTile = mazeConfig.GetRandomTilePositionInside(mazeConfig.MovingWallSize + tilesToMove, 1);
			transformAspect.Position += new float3(randomTile.x, 0.0f, randomTile.y) + positionOffset;

			PostTransformMatrix postTransformMatrix = SystemAPI.GetComponent<PostTransformMatrix>(wallEntity);
			postTransformMatrix.Value = float4x4.Scale(mazeConfig.MovingWallSize, postTransformMatrix.Value.c1.y, postTransformMatrix.Value.c2.z);
			SystemAPI.SetComponent<PostTransformMatrix>(wallEntity, postTransformMatrix);

			MovingWall movingWall = SystemAPI.GetComponent<MovingWall>(wallEntity);
			movingWall.MoveSpeedInSeconds = UnityEngine.Random.Range(mazeConfig.MovingWallMinMoveSpeedInSeconds, mazeConfig.MovingWallMaxMoveSpeedInSeconds);
			movingWall.NumberOfTilesToMove = tilesToMove;
			movingWall.StartXIndex = randomTile.x;
			movingWall.CurrentXIndex = randomTile.x;
			movingWall.StartYIndex = randomTile.y;
			SystemAPI.SetComponent<MovingWall>(wallEntity, movingWall);

			spawnedMovingWalls[i] = movingWall;
		}

		// clear out walls for moving walls
		for(int i = 0; i < spawnedMovingWalls.Length; ++i)
        {
			int totalWidth = spawnedMovingWalls[i].NumberOfTilesToMove * 2 + mazeConfig.MovingWallSize;
			int startIndex = spawnedMovingWalls[i].StartXIndex - mazeConfig.MovingWallSize / 2 - spawnedMovingWalls[i].NumberOfTilesToMove + 1;

			for (int w = 0; w < totalWidth; ++w)
			{
				TileBufferElement upTile = tiles[Get1DIndex(startIndex + w, spawnedMovingWalls[i].StartYIndex + 1)];
				upTile.DownWall = false;
				tiles[Get1DIndex(startIndex + w, spawnedMovingWalls[i].StartYIndex + 1)] = upTile;

				TileBufferElement downTile = tiles[Get1DIndex(startIndex + w, spawnedMovingWalls[i].StartYIndex)];
				downTile.UpWall = false;
				tiles[Get1DIndex(startIndex + w, spawnedMovingWalls[i].StartYIndex)] = downTile;
			}
		}

		// Fixing tile borders
		for (int x = 0; x < mazeConfig.Width - 1; ++x)
		{
			for (int y = 0; y < mazeConfig.Height - 1; ++y)
			{
				TileBufferElement currentTile = tiles[Get1DIndex(x, y)];
				TileBufferElement rightTile = tiles[Get1DIndex(x + 1, y)];
				TileBufferElement upTile = tiles[Get1DIndex(x, y + 1)];

				if(currentTile.RightWall && !rightTile.LeftWall)
                {
					rightTile.LeftWall = true;
                }
				if(!currentTile.RightWall && rightTile.LeftWall)
                {
					currentTile.RightWall = true;
                }

				if (currentTile.UpWall && !upTile.DownWall)
				{
					upTile.DownWall = true;
				}
				if (!currentTile.UpWall && upTile.DownWall)
				{
					currentTile.UpWall = true;
				}

				tiles[Get1DIndex(x, y)] = currentTile;
				tiles[Get1DIndex(x + 1, y)] = rightTile;
				tiles[Get1DIndex(x, y + 1)] = upTile;
			}
		}

		for (int x = 0; x < mazeConfig.Width; x++)
		{
			for (int y = 0; y < mazeConfig.Height; y++)
			{
				TileBufferElement tile = tiles[Get1DIndex(x, y)];

				if (x == 0 || tile.LeftWall)
				{
					Entity wall = state.EntityManager.Instantiate(prefabConfig.WallPrefab);
					TransformAspect wallTransform = SystemAPI.GetAspectRW<TransformAspect>(wall);
					wallTransform.LocalPosition = new Vector3((x - .5f), wallTransform.LocalPosition.y, y);
					wallTransform.LocalRotation = Quaternion.Euler(0, 90, 0);
				}
				if (y == 0 || tile.DownWall)
				{
					Entity wall = state.EntityManager.Instantiate(prefabConfig.WallPrefab);
					TransformAspect wallTransform = SystemAPI.GetAspectRW<TransformAspect>(wall);
					wallTransform.LocalPosition = new Vector3(x, wallTransform.LocalPosition.y, y - .5f);
					wallTransform.LocalRotation = Quaternion.Euler(0, 0, 0);
				}
				if (x == mazeConfig.Width - 1)
				{
					Entity wall = state.EntityManager.Instantiate(prefabConfig.WallPrefab);
					TransformAspect wallTransform = SystemAPI.GetAspectRW<TransformAspect>(wall);
					wallTransform.LocalPosition = new Vector3((x + .5f), wallTransform.LocalPosition.y, y);
					wallTransform.LocalRotation = Quaternion.Euler(0, 90, 0);
				}
				if (y == mazeConfig.Height - 1)
				{
					Entity wall = state.EntityManager.Instantiate(prefabConfig.WallPrefab);
					TransformAspect wallTransform = SystemAPI.GetAspectRW<TransformAspect>(wall);
					wallTransform.LocalPosition = new Vector3(x, wallTransform.LocalPosition.y, y + .5f);
					wallTransform.LocalRotation = Quaternion.Euler(0, 0, 0);
				}
			}
		}

		// Use moving walls to re-set the tiles's walls
		for (int i = 0; i < spawnedMovingWalls.Length; ++i)
		{
			int startIndex = spawnedMovingWalls[i].StartXIndex - mazeConfig.MovingWallSize / 2 + 1;

			for (int w = 0; w < mazeConfig.MovingWallSize; ++w)
			{
				TileBufferElement upTile = tiles[Get1DIndex(startIndex + w, spawnedMovingWalls[i].StartYIndex + 1)];
				upTile.DownWall = true;
				tiles[Get1DIndex(startIndex + w, spawnedMovingWalls[i].StartYIndex + 1)] = upTile;

				TileBufferElement downTile = tiles[Get1DIndex(startIndex + w, spawnedMovingWalls[i].StartYIndex)];
				downTile.UpWall = true;
				tiles[Get1DIndex(startIndex + w, spawnedMovingWalls[i].StartYIndex)] = downTile;
			}
		}

		// To Use
		// SystemAPI.GetSingletonBuffer<TileBufferElement>();

		state.Enabled = false;
    }
}
