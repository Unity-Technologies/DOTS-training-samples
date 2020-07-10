using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AutoFarmers
{
    class AttackSystem : SystemBase
    {
        private EntityQuery attackingQuery;
        private EntityCommandBufferSystem ecbSystem;

        protected override void OnCreate()
        {
            attackingQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new [] {ComponentType.ReadWrite<Attacking>()},
                None = new [] {ComponentType.ReadWrite<PathFindingTarget>()}
            });
            ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var attackingEntities =
                attackingQuery.ToEntityArrayAsync(Allocator.TempJob, out var attackingEntitiesDependency);
            var attackingArray = attackingQuery.ToComponentDataArrayAsync<Attacking>(Allocator.TempJob, out var attackingArrayDependency);
            Dependency = JobHandle.CombineDependencies(Dependency, attackingArray.SortJob(attackingArrayDependency), attackingEntitiesDependency);
            
            var cellPositionAccessor = GetComponentDataFromEntity<CellPosition>(true);
            var cellSizeAccessor = GetComponentDataFromEntity<CellSize>(true);
            var healthAccessor = GetComponentDataFromEntity<Health>();
            var deltaTime = Time.DeltaTime;
            var ecb = ecbSystem.CreateCommandBuffer();
            var gridEntity = GetSingletonEntity<Grid>();
            var grid = GetSingleton<Grid>();
            var cellEntityBuffer = EntityManager.GetBuffer<CellEntityElement>(gridEntity);
            
            Job.WithReadOnly(cellPositionAccessor).WithReadOnly(cellSizeAccessor).WithReadOnly(cellEntityBuffer).WithCode(() =>
            {
                var i = 0;
                while (i < attackingArray.Length)
                {
                    var firstIndex = i;
                    var target = attackingArray[i].Target;
                    var attackers = 0;
                    do
                    {
                        attackers += 1;
                        i += 1;
                    } while (i < attackingArray.Length && attackingArray[i].Target == target);

                    if (!healthAccessor.Exists(target))
                    {
                        for (var j = firstIndex; j < i; j++)
                        {
                            ecb.RemoveComponent<Attacking>(attackingEntities[j]);
                        }
                        continue;
                    }

                    var damage = attackers * deltaTime * 0.1f;
                    var health = healthAccessor[target].Value - damage;
                    
                    if (health < 0f)
                    {
                        for (var j = firstIndex; j < i; j++)
                        {
                            ecb.RemoveComponent<Attacking>(attackingEntities[j]);
                        }

                        var cellPosition = cellPositionAccessor[target].Value;
                        var cellSize = cellSizeAccessor[target].Value;
                        
                        for (var x = cellPosition.x; x < cellPosition.x + cellSize.x; x++)
                        for (var y = cellPosition.y; y < cellPosition.y + cellSize.y; y++)
                        {
                            var cellIndex = grid.GetIndexFromCoords(x, y);
                            var cellEntity = cellEntityBuffer[cellIndex].Value;
                            ecb.SetComponent(cellEntity, new Cell { Type = CellType.Raw });
                            ecb.SetComponent(cellEntity, new CellOccupant());
                        }
                        
                        ecb.DestroyEntity(target);
                    }
                    else
                    {
                        healthAccessor[target] = new Health { Value = health };
                    }
                }
            }).Schedule();
            
            Dependency = JobHandle.CombineDependencies(Dependency,
                attackingArray.Dispose(Dependency),
                attackingEntities.Dispose(Dependency));
            ecbSystem.AddJobHandleForProducer(Dependency);
            Dependency.Complete();
        }
    }
}