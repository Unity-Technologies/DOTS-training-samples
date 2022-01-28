using Combatbees.Testing.Maria;
using Unity.Entities;
using Unity.Mathematics;

public partial class Gravity : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SingletonMainScene>();
    }
    
    protected override void OnUpdate()
    {
        float resourceGravity = GetResourceGravity();

        Entities.WithAny<ResourceTag>().ForEach((ref Velocity velocity, in Holder holder) => {
            if (holder.Value == Entity.Null)
            {
                velocity.Value += new float3(0f, -resourceGravity, 0f); // Apply gravity to the resource
            }
        }).ScheduleParallel();
        Entities.WithAny<Falling>().ForEach((ref Velocity velocity) => 
        {
            velocity.Value += new float3(0f, -resourceGravity, 0f); // Apply gravity to the resource
        }).ScheduleParallel();
    }
    
    private float GetResourceGravity()
    {
        float resourceGravity = 0f;
        
        Entities.ForEach((in GravityConstants gravityConstants) =>
        {
            resourceGravity = gravityConstants.ResourceGravity;
        }).Run();

        return resourceGravity;
    } 
}
