using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    class GridInitializationSystem : SystemBase
    {
        private EntityQuery cellQuery;
        private EntityQuery farmQuery;
        
        protected override void OnCreate()
        {
            cellQuery = GetEntityQuery(ComponentType.ReadOnly<Cell>());
            
            // Create singleton entity
            var entity = EntityManager.CreateEntity(ComponentType.ReadOnly<Grid>());
            EntityManager.SetName(entity, "Grid");
            EntityManager.AddBuffer<CellTypeElement>(entity);
            EntityManager.AddBuffer<CellEntityElement>(entity);
            
            RequireForUpdate(farmQuery);
        }

        protected override void OnUpdate()
        {
            EntityManager.CompleteAllJobs();
            var entity = GetSingletonEntity<Grid>();
            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .WithChangeFilter<Farm>()
                .WithStoreEntityQueryInField(ref farmQuery)
                .WithNone<GridInitializedTag>()
                .ForEach((Entity farmEntity, Farm farm) =>
            {
                EntityManager.SetComponentData(entity, new Grid { Size = farm.MapSize });
                EntityManager.DestroyEntity(cellQuery);
                var cellCount = farm.MapSize.x * farm.MapSize.y;
                var cellEntities = EntityManager.Instantiate(farm.GroundPrefab, cellCount, Allocator.Temp);
                
                var typeBuffer = EntityManager.GetBuffer<CellTypeElement>(entity);
                typeBuffer.ResizeUninitialized(cellCount);
                for (var i = 0; i < cellCount; i++)
                {
                    typeBuffer[i] = new CellTypeElement(CellType.Raw);
                }
                
                var entityBuffer = EntityManager.GetBuffer<CellEntityElement>(entity);
                entityBuffer.ResizeUninitialized(cellCount);
                for (var i = 0; i < cellCount; i++)
                {
                    entityBuffer[i] = new CellEntityElement(cellEntities[i]);
                }
                
                for (var x = 0; x < farm.MapSize.x; x++)
                for (var y = 0; y < farm.MapSize.y; y++)
                {
                    var i = x * farm.MapSize.y + y;
                    EntityManager.SetComponentData(cellEntities[i], new Translation { Value = math.float3(x, 0, y) + new float3(0.5f, 0.0f, 0.5f)});
                    EntityManager.SetName(cellEntities[i], $"Cell {i} ({x}, {y})");
                }
                
                cellEntities.Dispose();
                EntityManager.AddComponent<GridInitializedTag>(farmEntity);
            }).Run();
        }
    }
}