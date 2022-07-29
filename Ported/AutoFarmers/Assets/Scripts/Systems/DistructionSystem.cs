using Unity.Collections;
using Unity.Entities;
using Unity.Entities.CodeGeneratedJobForEach;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(FarmerTargetLocatorSystem))]
public partial class DistructionSystem : SystemBase
    {
        private EntityQuery attackingQuery;
        private EntityQuery attackingQuery2;
        private EntityCommandBufferSystem ecbSystem;

        protected override void OnCreate()
        {
            attackingQuery = GetEntityQuery(ComponentType.ReadWrite<Distruction>());
            ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            
        }

        protected override void OnUpdate()
        {
            var attackingEntities =
                attackingQuery.ToEntityListAsync(Allocator.TempJob, out var attackingEntitiesDependency);
            var attackingArray = attackingQuery.ToComponentDataListAsync<Distruction>(Allocator.TempJob, out var attackingArrayDependency);
            Dependency = JobHandle.CombineDependencies(Dependency, (attackingArrayDependency), attackingEntitiesDependency);
            
             var healthAccessor = GetComponentDataFromEntity<Health>();
            var deltaTime = Time.DeltaTime;
            var ecb = ecbSystem.CreateCommandBuffer();
           
            Job.WithCode(() =>
            {
                // Debug.Log(attackingArray.Length);
                if (attackingArray.Length == 0)
                {
                    return;
                }
            
                var i = 0;
                while (i < attackingArray.Length)
                {
                    var firstIndex = i;
                    var target = attackingArray[i].Target;
                    i += 1;
                    if (healthAccessor.HasComponent(target))
                    {
                        var attackers = 1;
                        while (i < attackingArray.Length && attackingArray[i].Target == target)
                        {
                            attackers += 1;
                            i += 1;
                        }

                        var damage = attackers * deltaTime * 0.1f;
                        var health = healthAccessor[target].Value - damage;

                        if (health < 0f)
                        {
                            for (var j = firstIndex; j < i; j++)
                            {
                                ecb.RemoveComponent<Distruction>(attackingEntities[j]);
                            }

                            ecb.DestroyEntity(target);
                            
                        }
                        else
                        {
                            healthAccessor[target] = new Health {Value = health};
                        }
                    }
                }
            }).Run();
            Dependency = JobHandle.CombineDependencies(Dependency,
                attackingArray.Dispose(Dependency),
                attackingEntities.Dispose(Dependency));
            ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
