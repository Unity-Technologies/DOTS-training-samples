using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(TerrainAreaSystem))]
public partial class TankSpawnerSystem : SystemBase
{
	private EntityQuery _tankQuery;
	private EntityQuery _brickQuery;

    protected override void OnCreate()
    {
		_tankQuery = GetEntityQuery(typeof(Tank));
		_brickQuery = GetEntityQuery(typeof(Brick), typeof(Translation));
	}

    protected override void OnUpdate()
    {
		if (_tankQuery.IsEmpty)
        {
			CreateTanks();
		}
    }

	private void ShuffleList<T>(NativeArray<T> list, Unity.Mathematics.Random dice) where T: struct
	{
		int index = list.Length;
		while (0 != index)
		{
			int randomIndex = dice.NextInt(0, index);
			index--;
			T temp = list[index];
			list[index] = list[randomIndex];
			list[randomIndex] = temp;
		}
	}

	void ClearTanks()
    {

    }

	public void CreateTanks()
	{

		ClearTanks();

		// Create query for EntityPrefabHolder
		var ecb = new EntityCommandBuffer(Allocator.Temp);
		var brickEntities = _brickQuery.ToEntityArray(Allocator.Temp);

		// The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
		// and can be used in jobs. For simplicity and debuggability in development,
		// we'll initialize it with a constant. (In release, we'd want a seed that
		// randomly varies, such as the time from the user's system clock.)
		var random = new Unity.Mathematics.Random(234);

		ShuffleList(brickEntities, random);

		var bricks = _brickQuery.ToComponentDataArray<Brick>(brickEntities, Allocator.Temp);
		var brickPositions = _brickQuery.ToComponentDataArray<Translation>(brickEntities, Allocator.Temp);

		var gridEntity = GetSingletonEntity<OccupiedElement>();
		var occupiedGrid = GetBuffer<OccupiedElement>(gridEntity);
		var brickGrid = GetBuffer<EntityElement>(gridEntity);
		var terrainData = GetSingleton<TerrainData>();
		var player = GetSingletonEntity<PlayerTag>();

		Entities
			.ForEach((Entity entity, in EntityPrefabHolder prefabHolder, in TankData tankData) =>
				{
					int maxBrickCount = terrainData.TerrainWidth * terrainData.TerrainLength;
					int tankCount = math.min(maxBrickCount-1, tankData.TankCount);

					for (int i=0; i<tankCount; i++)
                    {
						var brick = bricks[i];
						var brickTranslation = brickPositions[i];
						
						var instance = ecb.Instantiate(prefabHolder.TankEntityPrefab);
						float3 position = brickTranslation.Value;
						position.y += brick.height / 2f + Constants.TANK_Y_OFFSET;
						ecb.SetComponent(instance, new Translation
						{
							Value = position
						});

						int2 gridPosition = TerrainUtility.BoxFromLocalPosition(position, terrainData.TerrainWidth, terrainData.TerrainLength);
						int index = gridPosition.x + gridPosition.y * terrainData.TerrainWidth;
						occupiedGrid[index] = true;
					}

					// Position the player at the start position
					int startBrickIndex = maxBrickCount / 2;
					
					while (occupiedGrid[startBrickIndex])
                    {
						startBrickIndex = (startBrickIndex+1) % maxBrickCount;
                    }
					// Hopefully we should exit...

					var centerBrick = brickGrid[startBrickIndex];
					var playerTranslation = GetComponent<Translation>(centerBrick).Value;
					// Origin is at 0.5 y when unscaled.
					playerTranslation.y *= 2f;
					playerTranslation.y += Constants.PLAYER_Y_OFFSET;
					ecb.SetComponent(player, new Translation { Value = playerTranslation });

					float maxHeight = playerTranslation.y + Constants.BOUNCE_HEIGHT;
					var parabola = Parabola.Create(playerTranslation.y, maxHeight, playerTranslation.y);
					parabola.startPoint = playerTranslation.xz;
					parabola.endPoint = playerTranslation.xz;
					parabola.duration = Constants.BOUNCE_BASE_DURATION;
					ecb.SetComponent(player, parabola);
				}).Run();

		brickEntities.Dispose();
		bricks.Dispose();
		brickPositions.Dispose();

		ecb.Playback(EntityManager);
		ecb.Dispose();
	}

}
