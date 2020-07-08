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
        
        protected override void OnCreate()
        {
            cellQuery = GetEntityQuery(ComponentType.ReadOnly<Cell>());
        }

        protected override void OnUpdate()
        {
            // Make sure we only have 1 farm
            GetSingletonEntity<Farm>();
            
            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .WithChangeFilter<Farm>()
                .WithNone<CellTypeElement>()
                .ForEach((Entity entity, Farm farm) =>
            {
                EntityManager.DestroyEntity(cellQuery);
                var cellCount = farm.MapSize.x * farm.MapSize.y;
                var cellEntities = EntityManager.Instantiate(farm.GroundPrefab, cellCount, Allocator.Temp);
                
                var typeBuffer = EntityManager.AddBuffer<CellTypeElement>(entity);
                typeBuffer.ResizeUninitialized(cellCount);
                for (var i = 0; i < cellCount; i++)
                {
                    typeBuffer[i] = new CellTypeElement(CellType.Raw);
                }
                
                var entityBuffer = EntityManager.AddBuffer<CellEntityElement>(entity);
                entityBuffer.ResizeUninitialized(cellCount);
                for (var i = 0; i < cellCount; i++)
                {
                    entityBuffer[i] = new CellEntityElement(cellEntities[i]);
                }
                
                for (var x = 0; x < farm.MapSize.x; x++)
                for (var y = 0; y < farm.MapSize.y; y++)
                {
                    var i = x * farm.MapSize.y + y;
                    EntityManager.SetComponentData(cellEntities[i], new Translation { Value = math.float3(x, 0, y)});
                }
                
                cellEntities.Dispose();
            }).Run();
        }
    }
}