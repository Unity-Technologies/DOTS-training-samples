using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace CombatBees.Testing.BeeFlight
{
    public partial class ResourceMovement : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
            RequireSingletonForUpdate<ListSingelton>();
        }
        
        protected override void OnUpdate()
        {
            var allTranslations = GetComponentDataFromEntity<Translation>(true);
            
            // Building a list of (Bee, Resource) pairs
            NativeList<(Entity, Entity)> beeResourcePairs = new NativeList<(Entity, Entity)>(Allocator.TempJob);
            
            Entities.WithAll<Bee>().ForEach((Entity entity, int entityInQueryIndex,ref IsHoldingResource isHoldingResource, in BeeTargets beeTargets) =>
            {
                Entity beeResourceTarget = beeTargets.ResourceTarget;
                // if bee is holding a resource add (Bee Entity, Resource Entity) pair to the list
                if (isHoldingResource.Value)
                {
                    beeResourcePairs.Add((entity, beeResourceTarget));
                }
            }).Run();

            float deltaTime = Time.DeltaTime;
            
            Entities.WithAll<Resource>().ForEach(
                (Entity entity, ref Translation translation, ref Holder holder) =>
                {
                    foreach (var beeResourcePair in beeResourcePairs)
                    {
                        if (entity == beeResourcePair.Item2)
                        { 
                            // Move resource to the bee's position
                            translation.Value = allTranslations[beeResourcePair.Item1].Value;
                        }
                        else
                        {
                            // Apply gravity
                            if (translation.Value.y > 0.5f)
                            {
                                translation.Value.y -= 3.0f * deltaTime;
                            } else if (translation.Value.y < 0.5f)
                            {
                                translation.Value.y = 0.5f;
                            }
                        }
                    }
                }).Run();

            beeResourcePairs.Dispose();
        }
    }
}