using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TileCreationSystem : ISystem
{
    public int Width;
    public int Height;

    public void OnCreate(ref SystemState state)
    {
        Width = 100;
        Height = 100;
        state.RequireForUpdate<PrefabConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

	public int Get1DIndex(int xIndex, int yIndex)
    {
		return xIndex + yIndex * Width;
    }

	public void CreateGround(ref SystemState state)
    {
		PrefabConfig prefabConfig = SystemAPI.GetSingleton<PrefabConfig>();

		Entity groundEntity = state.EntityManager.Instantiate(prefabConfig.TilePrefab);
		TransformAspect transformAspect = SystemAPI.GetAspectRW<TransformAspect>(groundEntity);
		transformAspect.Position += new float3(Width * 0.5f - 0.5f, 0.0f, Height * 0.5f - 0.5f);

		PostTransformMatrix postTransformMatrix = SystemAPI.GetComponent<PostTransformMatrix>(groundEntity);
		postTransformMatrix.Value = float4x4.Scale(Width, postTransformMatrix.Value.c1.y, Height);
		SystemAPI.SetComponent<PostTransformMatrix>(groundEntity, postTransformMatrix);
	}

    public void OnUpdate(ref SystemState state)
    {
		CreateGround(ref state);

        PrefabConfig prefabConfig = SystemAPI.GetSingleton<PrefabConfig>();
			
        Entity entity = state.EntityManager.CreateEntity();
        DynamicBuffer<TileBufferElement> tiles = state.EntityManager.AddBuffer<TileBufferElement>(entity);
        tiles.Capacity = Width * Height;

        TileBufferElement tileBufferElement = new TileBufferElement
        {
            UpWall = true,
            DownWall = true,
            RightWall = true,
            LeftWall = true
        };

        for (int i = 0; i < Width * Height; ++i)
        {
			tiles.Add(tileBufferElement);
        }

		Stack<Vector2Int> stack = new Stack<Vector2Int>();
		Vector2Int current = new Vector2Int(UnityEngine.Random.Range(0, Width), UnityEngine.Random.Range(0, Height));
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
			if (current.x < Width - 1 && !tiles[Get1DIndex(current.x + 1, current.y)].TempVisited)
				unvisitedNeighbors.Add(new Vector2Int(current.x + 1, current.y));
			if (current.y < Height - 1 && !tiles[Get1DIndex(current.x, current.y + 1)].TempVisited)
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

	//	GameObject groundTile = Instantiate(tilePrefab, transform);
	//	groundTile.transform.localPosition = new Vector3(TILE_SPACING * (width - 1) / 2, groundTile.transform.localPosition.y - .01f, TILE_SPACING * (length - 1) / 2);
		//groundTile.transform.localScale = new Vector3(TILE_SPACING * width, groundTile.transform.localScale.y, TILE_SPACING * length);
		//tileGameObjects.Add(groundTile);


		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				TileBufferElement tile = tiles[Get1DIndex(x, y)];

				if (tile.LeftWall)
				{
					Entity wall = state.EntityManager.Instantiate(prefabConfig.WallPrefab);
					TransformAspect wallTransform = SystemAPI.GetAspectRW<TransformAspect>(wall);
					wallTransform.LocalPosition = new Vector3((x - .5f), wallTransform.LocalPosition.y, y);
					wallTransform.LocalRotation = Quaternion.Euler(0, 90, 0);
				}
				if (tile.DownWall)
				{
					Entity wall = state.EntityManager.Instantiate(prefabConfig.WallPrefab);
					TransformAspect wallTransform = SystemAPI.GetAspectRW<TransformAspect>(wall);
					wallTransform.LocalPosition = new Vector3(x, wallTransform.LocalPosition.y, y - .5f);
					wallTransform.LocalRotation = Quaternion.Euler(0, 0, 0);
				}
				if (x == Width - 1 && tile.RightWall)
				{
					Entity wall = state.EntityManager.Instantiate(prefabConfig.WallPrefab);
					TransformAspect wallTransform = SystemAPI.GetAspectRW<TransformAspect>(wall);
					wallTransform.LocalPosition = new Vector3((x + .5f), wallTransform.LocalPosition.y, y);
					wallTransform.LocalRotation = Quaternion.Euler(0, 90, 0);
				}
				if (y == Height - 1 && tile.UpWall)
				{
					Entity wall = state.EntityManager.Instantiate(prefabConfig.WallPrefab);
					TransformAspect wallTransform = SystemAPI.GetAspectRW<TransformAspect>(wall);
					wallTransform.LocalPosition = new Vector3(x, wallTransform.LocalPosition.y, y + .5f);
					wallTransform.LocalRotation = Quaternion.Euler(0, 0, 0);
				}
			}
		}



		// To Use
		// SystemAPI.GetSingletonBuffer<TileBufferElement>();

		state.Enabled = false;
    }
}
