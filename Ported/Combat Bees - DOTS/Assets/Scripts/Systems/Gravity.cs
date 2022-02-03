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
        float resourceGravity = GetSingleton<GravityConstants>().ResourceGravity;

        Entities.WithAny<ResourceTag>().ForEach((ref Velocity velocity, in Holder holder) => {
            if (holder.Value == Entity.Null)
            {
                velocity.Value += new float3(0f, -resourceGravity, 0f); // Apply gravity to the resource
            }
        }).ScheduleParallel();
        Entities.ForEach((ref Velocity velocity,ref Falling falling) => 
        {
            if(falling.shouldFall)
                velocity.Value += new float3(0f, -resourceGravity, 0f); // Apply gravity to the resource
        }).ScheduleParallel();
    }
}
