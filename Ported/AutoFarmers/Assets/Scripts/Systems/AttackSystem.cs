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
            attackingQuery = GetEntityQuery(ComponentType.ReadWrite<Attacking>());
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
            var cellTypeBuffer = EntityManager.GetBuffer<CellTypeElement>(gridEntity);
            
            Job.WithReadOnly(cellPositionAccessor).WithReadOnly(cellSizeAccessor).WithCode(() =>
            {
                if (attackingArray.Length == 0)
                {
                    return;
                }

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
                        
                        for (var x = cellPosition.x; x < cellSize.x; x++)
                        for (var y = cellPosition.y; y < cellSize.y; y++)
                        {
                            var cellIndex = x * grid.Size.y + y;
                            cellTypeBuffer[cellIndex] = new CellTypeElement(CellType.Raw);
                        }
                        
                        ecb.DestroyEntity(target);
                    }
                    else
                    {
                        healthAccessor[target] = new Health { Value = health };
                    }
                }
            }).Run();
            
            Dependency = JobHandle.CombineDependencies(Dependency,
                attackingArray.Dispose(Dependency),
                attackingEntities.Dispose(Dependency));
            ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}