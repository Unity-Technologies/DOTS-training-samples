using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace AutoFarmers
{
    public struct Pathing : IDisposable
    {
		static readonly int[] dirsX = new int[] { 1, -1, 0, 0 };
		static readonly int[] dirsY = new int[] { 0, 0, 1, -1 };
		int2 mapSize;
		[ReadOnly] NativeArray<Entity> tiles;
		[ReadOnly] ComponentDataFromEntity<GroundTile> groundTileFromEntity;
		NativeArray<int> visitedTiles;
		NativeList<int> activeTiles;
		NativeList<int> nextTiles;
		NativeList<int> outputTiles;
		public RectInt FullMapZone => new RectInt(0, 0, mapSize.x, mapSize.y);


		public int Hash(int x, int y) => Hash(new int2(x, y));
		public int Hash(int2 position) => position.x + mapSize.x * position.y;
		public void Unhash(int hash, out int x, out int y)
		{
			y = hash / mapSize.x;
			x = hash - y * mapSize.x;
		}
		public int2 Unhash(int hash)
		{
			Unhash(hash, out var x, out var y);
			return new int2(x, y);
		}
		public GroundTile Tile(int x, int y) => Tile(new int2(x, y));
		public GroundTile Tile(int2 position) => Tile(Hash(position));
		GroundTile Tile(int hash) => groundTileFromEntity[tiles[hash]];

		public readonly struct IsNavigableDefault : ITilePredicate
        {
            public bool Predicate(GroundTile tile)
            {
				return tile.TileRock == Entity.Null;
            }
        }

		public readonly struct IsNavigableAll : ITilePredicate
		{
			public bool Predicate(GroundTile tile)
			{
				return true;
			}
		}
		public readonly struct IsRock : ITilePredicate
		{
			public bool Predicate(GroundTile tile)
			{
				return tile.TileRock != Entity.Null;
			}
		}

		public readonly struct IsStore : ITilePredicate
		{
			public bool Predicate(GroundTile tile)
			{
				return tile.StoreTile;
			}
		}

		public readonly struct IsTillable : ITilePredicate
		{
			public bool Predicate(GroundTile tile)
			{
				return tile.State == GroundState.Default;
			}
		}

		public readonly struct IsReadyForPlant : ITilePredicate
		{
			public bool Predicate(GroundTile tile)
			{
				return tile.State == GroundState.Tilled;
			}
		}

		public Pathing(in Farm farm,NativeArray<Entity> tiles,in ComponentDataFromEntity<GroundTile> groundTileFromEntity, Allocator allocator = Allocator.Temp)
        {
			mapSize = farm.MapSize;
            this.tiles = tiles;
			this.groundTileFromEntity = groundTileFromEntity;
			visitedTiles = new NativeArray<int>(farm.MapSize.x * farm.MapSize.y, allocator, NativeArrayOptions.UninitializedMemory);
			activeTiles = new NativeList<int>(allocator);
			nextTiles = new NativeList<int>(allocator);
			outputTiles = new NativeList<int>(allocator);
        }

        public Entity FindNearbyRock(int2 pos,int range, DynamicBuffer<Path> outputPath)
        {
			int rockPosHash = SearchForOne(pos, range, new IsNavigableDefault(),new IsRock(), FullMapZone);
			if (rockPosHash == -1)
			{
				return Entity.Null;
			}
			else
			{
				if (outputPath.IsCreated)
				{
					AssignLatestPath(outputPath, Unhash(rockPosHash));
				}
				return Tile(rockPosHash).TileRock;
			}
		}
		public void WalkTo<TCheckMatch>(int2 pos, int range, TCheckMatch CheckMatch, DynamicBuffer<Path> outputPath)
			where TCheckMatch : unmanaged, ITilePredicate
		{
			int storePosHash = SearchForOne(pos, range, new IsNavigableDefault(), CheckMatch, FullMapZone);
			if (storePosHash != -1)
			{
				if (outputPath.IsCreated)
				{
					AssignLatestPath(outputPath, Unhash(storePosHash));
				}
			}
		}
		public int SearchForOne<TNavigable,TCheckMatch>(int2 start, int range, TNavigable IsNavigable, TCheckMatch CheckMatch, RectInt requiredZone)
			where TNavigable:unmanaged,ITilePredicate
			where TCheckMatch:unmanaged,ITilePredicate
		{
			outputTiles = Search(start, range, IsNavigable, CheckMatch, requiredZone, 1);
			if (outputTiles.Length == 0)
			{
				return -1;
			}
			else
			{
				return outputTiles[0];
			}
		}

		public NativeList<int> Search<TNavigable, TCheckMatch>(int2 start, int range, TNavigable IsNavigable, TCheckMatch CheckMatch, RectInt requiredZone, int maxResultCount = 0)
			where TNavigable : unmanaged, ITilePredicate
			where TCheckMatch : unmanaged, ITilePredicate
		{
			var mapWidth = mapSize.x;
			var mapHeight = mapSize.y;
			for (int y = 0; y < mapHeight; y++)
			{
				for (int x = 0; x < mapWidth; x++)
				{
					visitedTiles[Hash(x,y)] = -1;
				}
			}
			outputTiles.Clear();
			visitedTiles[Hash(start)] = 0;
			activeTiles.Clear();
			nextTiles.Clear();
			nextTiles.Add(Hash(start));

			int steps = 0;

			while (nextTiles.Length > 0 && (steps < range || range == 0))
			{
				var temp = activeTiles;
				activeTiles = nextTiles;
				nextTiles = temp;
				nextTiles.Clear();

				steps++;

				for (int i = 0; i < activeTiles.Length; i++)
				{
					int x, y;
					Unhash(activeTiles[i], out x, out y);

					for (int j = 0; j < dirsX.Length; j++)
					{
						int x2 = x + dirsX[j];
						int y2 = y + dirsY[j];

						if (x2 < 0 || y2 < 0 || x2 >= mapWidth || y2 >= mapHeight)
						{
							continue;
						}

						if (visitedTiles[Hash(x2, y2)] == -1 || visitedTiles[Hash(x2, y2)] > steps)
						{

							int hash = Hash(x2, y2);
							if (IsNavigable.Predicate(Tile(x2,y2)))
							{
								visitedTiles[hash] = steps;
								nextTiles.Add(hash);
							}
							if (x2 >= requiredZone.xMin && x2 <= requiredZone.xMax)
							{
								if (y2 >= requiredZone.yMin && y2 <= requiredZone.yMax)
								{
									if (CheckMatch.Predicate(Tile(x2, y2)))
									{
										outputTiles.Add(hash);
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

			return outputTiles;
		}
		public void AssignLatestPath(DynamicBuffer<Path> target, int2 end)
		{
			target.Clear();
			var mapWidth = mapSize.x;
			var mapHeight = mapSize.y;
			var x = end.x;
			var y = end.y;
			target.Add(new int2(x,y));

			int dist = int.MaxValue;
			while (dist > 0)
			{
				int minNeighborDist = int.MaxValue;
				int bestNewX = x;
				int bestNewY = y;
				for (int i = 0; i < dirsX.Length; i++)
				{
					int x2 = x + dirsX[i];
					int y2 = y + dirsY[i];
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
				target.Add(new int2(x, y));
			}
		}

		public void Dispose()
        {
			visitedTiles.Dispose();
			activeTiles.Dispose();
			nextTiles.Dispose();
			outputTiles.Dispose();
        }

		public void Dispose(JobHandle inputDeps)
		{ 
			visitedTiles.Dispose(inputDeps);
			activeTiles.Dispose(inputDeps);
			nextTiles.Dispose(inputDeps);
			outputTiles.Dispose(inputDeps);
		}
		public struct StructualChange : IDisposable
		{
			int2 mapSize;
			Entity farmEntity;
			[ReadOnly] NativeArray<Entity> tiles;
			[ReadOnly] EntityManager entityManager;
			NativeArray<int> visitedTiles;
			NativeList<int> activeTiles;
			NativeList<int> nextTiles;
			NativeList<int> outputTiles;
			public RectInt FullMapZone => new RectInt(0, 0, mapSize.x, mapSize.y);


			public int Hash(int x, int y) => Hash(new int2(x, y));
			public int Hash(int2 position) => position.x + mapSize.x * position.y;
			public void Unhash(int hash, out int x, out int y)
			{
				y = hash / mapSize.x;
				x = hash - y * mapSize.x;
			}
			public int2 Unhash(int hash)
			{
				Unhash(hash, out var x, out var y);
				return new int2(x, y);
			}
			public GroundTile Tile(int x, int y) => Tile(new int2(x, y));
			public GroundTile Tile(int2 position) => Tile(Hash(position));
			GroundTile Tile(int hash) => entityManager.GetComponentData<GroundTile>(tiles[hash]);

			public readonly struct IsNavigableDefault : ITilePredicate
			{
				public bool Predicate(GroundTile tile)
				{
					return tile.TileRock == Entity.Null;
				}
			}

			public readonly struct IsNavigableAll : ITilePredicate
			{
				public bool Predicate(GroundTile tile)
				{
					return true;
				}
			}
			public readonly struct IsRock : ITilePredicate
			{
				public bool Predicate(GroundTile tile)
				{
					return tile.TileRock != Entity.Null;
				}
			}

			public readonly struct IsStore : ITilePredicate
			{
				public bool Predicate(GroundTile tile)
				{
					return tile.StoreTile;
				}
			}

			public readonly struct IsTillable : ITilePredicate
			{
				public bool Predicate(GroundTile tile)
				{
					return tile.State == GroundState.Default;
				}
			}

			public readonly struct IsReadyForPlant : ITilePredicate
			{
				public bool Predicate(GroundTile tile)
				{
					return tile.State == GroundState.Tilled;
				}
			}

			public StructualChange(Entity farmEntity, in EntityManager entityManager, Allocator allocator = Allocator.Temp)
			{
				this.farmEntity = farmEntity;
				var farm = entityManager.GetComponentData<Farm>(farmEntity);
				mapSize = farm.MapSize;
				tiles = entityManager.GetBuffer<Farm.GroundTiles>(farmEntity).Reinterpret<Entity>().AsNativeArray();
				this.entityManager = entityManager;
				visitedTiles = new NativeArray<int>(farm.MapSize.x * farm.MapSize.y, allocator, NativeArrayOptions.UninitializedMemory);
				activeTiles = new NativeList<int>(allocator);
				nextTiles = new NativeList<int>(allocator);
				outputTiles = new NativeList<int>(allocator);
			}

			public Entity FindNearbyRock(int2 pos, int range, DynamicBuffer<Path> outputPath)
			{
				int rockPosHash = SearchForOne(pos, range, new IsNavigableDefault(), new IsRock(), FullMapZone);
				if (rockPosHash == -1)
				{
					return Entity.Null;
				}
				else
				{
					if (outputPath.IsCreated)
					{
						AssignLatestPath(outputPath, Unhash(rockPosHash));
					}
					return Tile(rockPosHash).TileRock;
				}
			}
			public void WalkTo<TCheckMatch>(int2 pos, int range, TCheckMatch CheckMatch, DynamicBuffer<Path> outputPath)
				where TCheckMatch : unmanaged, ITilePredicate
			{
				int storePosHash = SearchForOne(pos, range, new IsNavigableDefault(), CheckMatch, FullMapZone);
				if (storePosHash != -1)
				{
					if (outputPath.IsCreated)
					{
						AssignLatestPath(outputPath, Unhash(storePosHash));
					}
				}
			}
			public int SearchForOne<TNavigable, TCheckMatch>(int2 start, int range, TNavigable IsNavigable, TCheckMatch CheckMatch, RectInt requiredZone)
				where TNavigable : unmanaged, ITilePredicate
				where TCheckMatch : unmanaged, ITilePredicate
			{
				outputTiles = Search(start, range, IsNavigable, CheckMatch, requiredZone, 1);
				if (outputTiles.Length == 0)
				{
					return -1;
				}
				else
				{
					return outputTiles[0];
				}
			}

			public NativeList<int> Search<TNavigable, TCheckMatch>(int2 start, int range, TNavigable IsNavigable, TCheckMatch CheckMatch, RectInt requiredZone, int maxResultCount = 0)
				where TNavigable : unmanaged, ITilePredicate
				where TCheckMatch : unmanaged, ITilePredicate
			{
				var mapWidth = mapSize.x;
				var mapHeight = mapSize.y;
				for (int y = 0; y < mapHeight; y++)
				{
					for (int x = 0; x < mapWidth; x++)
					{
						visitedTiles[Hash(x, y)] = -1;
					}
				}
				tiles = entityManager.GetBuffer<Farm.GroundTiles>(farmEntity).Reinterpret<Entity>().AsNativeArray();
				outputTiles.Clear();
				visitedTiles[Hash(start)] = 0;
				activeTiles.Clear();
				nextTiles.Clear();
				nextTiles.Add(Hash(start));

				int steps = 0;

				while (nextTiles.Length > 0 && (steps < range || range == 0))
				{
					var temp = activeTiles;
					activeTiles = nextTiles;
					nextTiles = temp;
					nextTiles.Clear();

					steps++;

					for (int i = 0; i < activeTiles.Length; i++)
					{
						int x, y;
						Unhash(activeTiles[i], out x, out y);

						for (int j = 0; j < dirsX.Length; j++)
						{
							int x2 = x + dirsX[j];
							int y2 = y + dirsY[j];

							if (x2 < 0 || y2 < 0 || x2 >= mapWidth || y2 >= mapHeight)
							{
								continue;
							}

							if (visitedTiles[Hash(x2, y2)] == -1 || visitedTiles[Hash(x2, y2)] > steps)
							{

								int hash = Hash(x2, y2);
								if (IsNavigable.Predicate(Tile(x2, y2)))
								{
									visitedTiles[hash] = steps;
									nextTiles.Add(hash);
								}
								if (x2 >= requiredZone.xMin && x2 <= requiredZone.xMax)
								{
									if (y2 >= requiredZone.yMin && y2 <= requiredZone.yMax)
									{
										if (CheckMatch.Predicate(Tile(x2, y2)))
										{
											outputTiles.Add(hash);
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

				return outputTiles;
			}
			public void AssignLatestPath(DynamicBuffer<Path> target, int2 end)
			{
				target.Clear();
				var mapWidth = mapSize.x;
				var mapHeight = mapSize.y;
				var x = end.x;
				var y = end.y;
				target.Add(new int2(x, y));

				int dist = int.MaxValue;
				while (dist > 0)
				{
					int minNeighborDist = int.MaxValue;
					int bestNewX = x;
					int bestNewY = y;
					for (int i = 0; i < dirsX.Length; i++)
					{
						int x2 = x + dirsX[i];
						int y2 = y + dirsY[i];
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
					target.Add(new int2(x, y));
				}
			}

			public void Dispose()
			{
				visitedTiles.Dispose();
				activeTiles.Dispose();
				nextTiles.Dispose();
				outputTiles.Dispose();
			}

			public void Dispose(JobHandle inputDeps)
			{
				visitedTiles.Dispose(inputDeps);
				activeTiles.Dispose(inputDeps);
				nextTiles.Dispose(inputDeps);
				outputTiles.Dispose(inputDeps);
			}

		}
	}

}