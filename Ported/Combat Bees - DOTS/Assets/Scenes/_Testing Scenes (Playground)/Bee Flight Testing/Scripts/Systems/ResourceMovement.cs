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

            var deltaTime = Time.DeltaTime;
            float gravity = 4.0f;
            float halfResourceHeight = 0.5f;
            
            Entities.WithAll<Resource>().ForEach((Entity entity, ref Translation translation, ref Holder holder) =>
            {
                bool resourceHeld = false;
                float3 holderPosition = float3.zero;
                
                foreach (var beeResourcePair in beeResourcePairs)
                {
                    if (entity == beeResourcePair.Item2) // Item2 = Resource
                    {
                        resourceHeld = true;
                        holderPosition = allTranslations[beeResourcePair.Item1].Value; // Item1 = Bee
                        break;
                    }
                }

                if (resourceHeld)
                {
                    // Move the resource to the bee's position
                    translation.Value = holderPosition + holder.Offset;
                }
                else
                {
                    if (translation.Value.y > halfResourceHeight) // If above ground
                    {
                        // Apply gravity
                        translation.Value.y -= gravity * deltaTime;
                    } else if (translation.Value.y < halfResourceHeight) // if below ground
                    {
                        translation.Value.y = halfResourceHeight; // put on the ground
                    }
                }
            }).Run();

            beeResourcePairs.Dispose();
        }
    }
}