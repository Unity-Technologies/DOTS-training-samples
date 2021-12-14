using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace CombatBees.Testing.BeeFlight
{
    public partial class BeeTargetingSystem : SystemBase
    {
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
 
            Entities.WithAll<Bee>().ForEach((ref BeeTargets beeTargets) =>
            {
                if (beeTargets.ResourceTarget == Entity.Null)
                {
                    beeTargets.ResourceTarget = randomResource;
                }
            }).ScheduleParallel();
        }
    }
}