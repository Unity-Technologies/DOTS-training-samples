using Unity.Entities;
using Unity.Transforms;

namespace AutoFarmers
{
    class FindRockToSmashSystem : SystemBase
    {
        private EntityCommandBufferSystem ecbSystem;
        
        protected override void OnCreate()
        {
            ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = ecbSystem.CreateCommandBuffer();
            var gridEntity = GetSingletonEntity<Grid>();
            var grid = GetSingleton<Grid>();
            var cellTypeBuffer = EntityManager.GetBuffer<CellTypeElement>(gridEntity);
            var cellEntityBuffer = EntityManager.GetBuffer<CellEntityElement>(gridEntity);
            var occupantAccessor = GetComponentDataFromEntity<CellOccupant>();
            
            Entities.WithAll<SmashRock_Intent>().WithNone<Attacking>().WithNone<Target>()
                .WithReadOnly(cellTypeBuffer).WithReadOnly(cellEntityBuffer)
                .WithStructuralChanges()
                .ForEach((Entity entity, Translation translation) =>
                {
                var cellPosition = CellPosition.FromTranslation(translation.Value);
                var dx = 1;
                var dy = 0;
                var segmentLength = 1;
                var segmentPassed = 0;
                for (var k = 0; k < 256; k++)
                {
                    cellPosition.x += dx;
                    cellPosition.y += dy;
                    segmentPassed++;

                    if (segmentPassed == segmentLength)
                    {
                        segmentPassed = 0;
                        int temp = dx;
                        dx = -dy;
                        dy = temp;

                        if (dy == 0)
                        {
                            segmentLength++;
                        }
                    }

                    Entity? nearbyRock = null;
                    for (var x = -1; x <= 1; x += 2)
                    for (var y = -1; y <= 1; y += 2)
                    {
                        var cellIndex = (cellPosition.x + x) * grid.Size.y + (cellPosition.y + y);
                        if (cellTypeBuffer[cellIndex].Value == CellType.Rock)
                        {
                            var cellEntity = cellEntityBuffer[cellIndex].Value;
                            nearbyRock = occupantAccessor[cellEntity].Value;
                            break;
                        }
                    }
                    
                    if (nearbyRock.HasValue)
                    {
                        var cellIndex = cellPosition.y * grid.Size.x + cellPosition.x;
                        var cellEntity = cellEntityBuffer[cellIndex].Value;
                        ecb.AddComponent(entity, new Target
                        {
                            Value = cellEntity
                        });
                        ecb.AddComponent(entity, new Attacking
                        {
                            Target = nearbyRock.Value
                        });
                        return;
                    }
                }

                ecb.RemoveComponent<SmashRock_Intent>(entity);
            }).Run();
            ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}