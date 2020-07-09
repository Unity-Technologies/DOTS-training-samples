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
            
            var healthAccessor = GetComponentDataFromEntity<Health>();
            var deltaTime = Time.DeltaTime;
            var ecb = ecbSystem.CreateCommandBuffer();
            
            Job.WithCode(() =>
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
                    var health = healthAccessor[target].Value;
                    health -= damage;
                    if (health < 0f)
                    {
                        for (var j = firstIndex; j < i; j++)
                        {
                            ecb.RemoveComponent<Attacking>(attackingEntities[j]);
                        }
                        ecb.DestroyEntity(target);

                        health = 0f;
                    }
                    healthAccessor[target] = new Health { Value = health };
                }
            }).Schedule();
            
            Dependency = attackingArray.Dispose(Dependency);
            Dependency = attackingEntities.Dispose(Dependency);
            ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}