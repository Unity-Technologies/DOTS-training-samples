using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FireGridInit : SystemBase
{

    NativeArray<FireGridCell> StarData;
    private EntityQuery m_GridQuery;


    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BucketBrigadeConfig>();

        m_GridQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadOnly<FireGrid>() },
            });
    }

    protected override void OnUpdate()
    {
        if(m_GridQuery.CalculateChunkCount()>0)
        {
            return;
        }

        var config = GetSingleton<BucketBrigadeConfig>();
        var prefabs = GetSingleton<SpawnerConfig>();


        int Count = config.GridDimensions.x * config.GridDimensions.y;

        var ArchetypeGrid = EntityManager.CreateArchetype(typeof(FireGrid));
        //var ArchetypeCell = EntityManager.CreateArchetype(typeof(GridCellIndex), typeof(CubeColor));


        var firegrid = EntityManager.CreateEntity(ArchetypeGrid);
        var buffer = EntityManager.AddBuffer<FireGridCell>(firegrid);
        //buffer.Capacity

        StarData = new NativeArray<FireGridCell>(Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);

        Random rand = new Random();
        rand.InitState();

        for (int i = 0; i < config.StartingFireCount; i++)
        {
            FireGridCell fire = new FireGridCell();
            fire.Temperature = rand.NextFloat(config.Flashpoint, 1.0f);
            StarData[rand.NextInt(Count - 1)] = fire;
        }

        buffer.AddRange(StarData);

        for (int i = 0; i < Count; ++i)
        {
            var cell = EntityManager.Instantiate(prefabs.FireCellPrefab);
            var position = GetComponent<Translation>(cell);
            int cellRowIndex = i / config.GridDimensions.x;
            int cellColumnIndex = i % config.GridDimensions.x;


            position.Value = new float3(cellColumnIndex * config.CellSize, 0, cellRowIndex * config.CellSize);
            SetComponent(cell, position);

            var scale = GetComponent<NonUniformScale>(cell);
            scale.Value.y = StarData[i].Temperature +0.001f;
            SetComponent(cell, scale);

            var index = GetComponent<GridCellIndex>(cell);
            index.Index = i;

        }

        this.Enabled = false;
    }

    protected override void OnDestroy() 
    {
        base.OnDestroy();
        StarData.Dispose();
    }
}
