 
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


    public partial class MapCreationSystem : SystemBase
    {
         EntityQuery cellQuery;

         protected override void OnCreate()
        {
            cellQuery = GetEntityQuery(ComponentType.ReadOnly<Cell>());
            EntityManager.CreateEntity(ComponentType.ReadOnly<Grid>());
        }

      protected override void OnUpdate()
        {
            var entity = GetSingletonEntity<Grid>();
            Entities
                .WithStructuralChanges()
                .ForEach((in Map map) =>
                {
                    Grid grid = new Grid {size = map.mapSize};
                EntityManager.SetComponentData(entity, grid);
                EntityManager.DestroyEntity(cellQuery);
                var cellCount = map.mapSize.x * map.mapSize.y;
                var cellEntities = EntityManager.Instantiate(map.cellPrefab, cellCount, Allocator.Temp);

                for (var x = 0; x < map.mapSize.x; x++)
                {
                    for (var y = 0; y < map.mapSize.y; y++)
                    {
                        var i = x * map.mapSize.y + y;
                        EntityManager.SetComponentData(cellEntities[i], new Translation() { Value = math.float3(x, 0, y) + new float3(0.5f, 0.0f, 0.5f)});
                        EntityManager.SetComponentData(cellEntities[i], new CellPosition() { cellPosition = new int2(x, y)} );
                    }
                }
                cellEntities.Dispose();
            }).Run();
        }
    }