using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class ResourceMover : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SingletonMainScene>();
    }
    
    protected override void OnUpdate()
    {
        float3 containerMinPos = new float3();
        float3 containerMaxPos = new float3();
        
        Entities.ForEach((in Container container) =>
        {
            containerMinPos = container.MinPosition;
            containerMaxPos = container.MaxPosition;
        }).Run();
        
        float deltaTime = Time.DeltaTime;
        
        Entities.WithAll<ResourceTag>().ForEach((ref Translation translation, ref Velocity velocity, in Holder holder) => {
            if (holder.Value == Entity.Null)
            {
                float3 newPosition = translation.Value + velocity.Value * deltaTime;
                
                // Clamp the position to be within the container
                float3 clampedPosition = math.clamp(newPosition, containerMinPos, containerMaxPos);
        
                // If the resource hits the floor reset its velocity
                if (clampedPosition.y > newPosition.y) velocity.Value = float3.zero;
                
                // TODO: Check collisions with walls 
                
                translation.Value = clampedPosition;
            }
        }).ScheduleParallel();
    }
}
