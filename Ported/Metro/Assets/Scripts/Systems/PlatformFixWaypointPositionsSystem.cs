using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct PlatformFixWaypointPositionsSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Platform>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach (var(platform, transform, entity) in SystemAPI.Query<Platform, TransformAspect>().WithEntityAccess())
        {
            var newPlat = new Platform
            {
                WalkwayBackLower = transform.TransformPointLocalToWorld(platform.WalkwayBackLower), 
                WalkwayBackUpper = transform.TransformPointLocalToWorld(platform.WalkwayBackUpper), 
                WalkwayFrontLower = transform.TransformPointLocalToWorld(platform.WalkwayFrontLower), 
                WalkwayFrontUpper = transform.TransformPointLocalToWorld(platform.WalkwayFrontUpper), 
            };
            ecb.SetComponent(entity, newPlat);
        }
        state.Enabled = false;
    }
}
