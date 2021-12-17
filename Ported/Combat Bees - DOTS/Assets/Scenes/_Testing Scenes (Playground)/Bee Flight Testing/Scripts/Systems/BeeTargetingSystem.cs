using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Random = Unity.Mathematics.Random;

namespace CombatBees.Testing.BeeFlight
{
    public partial class BeeTargetingSystem : SystemBase
    {
        // TODO: Don't assign resources that have been already brought home
        
        private Random random;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
            random = new Random(123);
        }

        protected override void OnUpdate()
        {
            // Get all resources
            EntityQuery resourcesQuery = GetEntityQuery(typeof(Resource));
            NativeArray<Entity> allResourceEntities = resourcesQuery.ToEntityArray(Allocator.TempJob);

            // Copy resources from a Native Array to a List (for easy removal of items)
            List<Entity> resourceEntitiesList = new List<Entity>();

            foreach (var resourceEntity in allResourceEntities)
            {
                resourceEntitiesList.Add(resourceEntity);
            }
            
            // Initialize a random resource variable
            Entity randomResource = Entity.Null;

            
            List<Entity> assignedResources = new List<Entity>();
            
            Entities.WithAll<Bee>().ForEach((Entity entity, ref BeeTargets beeTargets) =>
            {
                if (beeTargets.ResourceTarget != Entity.Null)
                {
                    // Remove the already assigned resources from the list
                    resourceEntitiesList.Remove(beeTargets.ResourceTarget);
                }
            }).WithoutBurst().Run();

            Entities.WithAll<Bee>().ForEach((Entity entity, ref BeeTargets beeTargets) =>
            {
                if (beeTargets.ResourceTarget == Entity.Null && resourceEntitiesList.Count > 0)
                {
                    // Pick a random unassigned resource and assign it to a bee
                    int randomIndex = random.NextInt(resourceEntitiesList.Count);
                    randomResource = resourceEntitiesList[randomIndex];
                    beeTargets.ResourceTarget = randomResource;
                    resourceEntitiesList.RemoveAt(randomIndex);
                }
            }).WithoutBurst().Run();
            // if we use ScheduleParallel(), we can't write to variables defined outside of EFE
            
            // if we remove WithoutBurst(), we can't use "List<Entity>" (if we use "NativeList<Entity>" we get error
            // "The UNKNOWN_OBJECT_TYPE has been deallocated"

            allResourceEntities.Dispose();
        }
    }
}