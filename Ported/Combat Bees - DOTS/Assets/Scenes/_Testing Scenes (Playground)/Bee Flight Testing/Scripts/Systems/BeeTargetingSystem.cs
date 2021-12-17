using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace CombatBees.Testing.BeeFlight
{
    public partial class BeeTargetingSystem : SystemBase
    {
        // TODO: Extend to multiple bees + use DOTS random
        
        private Random random;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
            
            uint randomSeed = (uint) DateTime.Now.Millisecond;
            random = new Random(randomSeed);
        }

        protected override void OnUpdate()
        {
            EntityQuery resourcesQuery = GetEntityQuery(typeof(Resource));
            NativeArray<Entity> allResourceEntities = resourcesQuery.ToEntityArray(Allocator.Temp);
            Entity randomResource = allResourceEntities[random.NextInt(allResourceEntities.Length)];
            allResourceEntities.Dispose();

            Entity bee = Entity.Null;
            bool newTarget = false;
 
            Entities.WithAll<Bee>().ForEach((Entity entity, ref BeeTargets beeTargets) =>
            {
                if (beeTargets.ResourceTarget == Entity.Null)
                {
                    beeTargets.ResourceTarget = randomResource;
                    bee = entity;
                    newTarget = true;
                }
            }).WithoutBurst().Run(); // if we use ScheduleParallel(), the local variable bee is not assigned a new value
            
            Debug.Log(bee);
            
            Entities.WithAll<Resource>().ForEach((Entity entity, ref Holder holder) =>
            {
                if (entity == randomResource && newTarget)
                {
                    Debug.Log("bee assigned");
                    holder.Value = bee;
                }
            }).Run();
        }
    }
}