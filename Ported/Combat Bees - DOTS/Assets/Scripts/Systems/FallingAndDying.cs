using Combatbees.Testing.Maria;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class FallingAndDying : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SingletonMainScene>();
    }

    protected override void OnUpdate()
    {
        float3 containerMinPos = GetSingleton<Container>().MinPosition;
        float3 containerMaxPos = GetSingleton<Container>().MaxPosition;
        var deltaTime = Time.DeltaTime;
        
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities.WithAll<Falling>().ForEach((Entity entity,ref Translation translation ,ref Velocity velocity,ref Falling falling) => {
        
            if (falling.shouldFall)
            {
                float3 newPosition = translation.Value + velocity.Value * deltaTime;
        
                // Clamp the position to be within the container
                float3 clampedPosition = math.clamp(newPosition, containerMinPos, containerMaxPos);
        
                // If the resource hits the floor reset its velocity
                if (clampedPosition.y > newPosition.y) velocity.Value = float3.zero;
        
                translation.Value = clampedPosition;
        
                if (0 <= falling.timeToLive)
                {
                    falling.timeToLive -= deltaTime;
                }
                else
                {
                    // ecb.DestroyEntity(entity);
                }
            }
        }).Run();
     
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
