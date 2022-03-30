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
		var terrainData = GetSingleton<TerrainData>();

		Entities
			.ForEach((Entity entity, in EntityPrefabHolder prefabHolder, in TankData tankData) =>
				{
					for (int i=0; i<tankData.TankCount; i++)
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

						int2 gridPosition = TerrainUtility.BoxFromLocalPosition(position);
						int index = gridPosition.x + gridPosition.y * terrainData.TerrainWidth;
						occupiedGrid[index] = true;
					}
				}).Run();

		brickEntities.Dispose();
		bricks.Dispose();
		brickPositions.Dispose();

		ecb.Playback(EntityManager);
		ecb.Dispose();
	}

}
