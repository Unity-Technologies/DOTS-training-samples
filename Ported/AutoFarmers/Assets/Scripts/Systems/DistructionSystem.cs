using Unity.Collections;
using Unity.Entities;
using Unity.Entities.CodeGeneratedJobForEach;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;


public partial class DistructionSystem : SystemBase
    {
        private EntityQuery attackingQuery;
        private EntityQuery attackingQuery2;
        private EntityCommandBufferSystem ecbSystem;

        protected override void OnCreate()
        {
            attackingQuery = GetEntityQuery(ComponentType.ReadWrite<Distruction>());
            attackingQuery2 = GetEntityQuery(ComponentType.ReadWrite<RockTag>());
            ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            
        }

        protected override void OnUpdate()
        {
            var rockPositionArray = attackingQuery2.ToComponentDataArray<RockTag>(Allocator.TempJob);
            Debug.Log(rockPositionArray.Length);
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
                             ecb.RemoveComponent<Distruction>(attackingEntities[j]);
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
