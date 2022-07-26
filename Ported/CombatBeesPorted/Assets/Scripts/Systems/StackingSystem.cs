using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
[WithAny(typeof(ResourceStateGrabbable), typeof(ResourceStateStacked), typeof(ResourceStateGrabbed))]
[WithAll(typeof(Gravity))]
[BurstCompile]
partial struct StackingJob : IJobEntity
{
    public float PlayVolumeFloor;
    public EntityCommandBuffer Ecb;
    void Execute(in Entity entity, ref Velocity vel, in Translation trans)
    {
        Ecb.SetComponentEnabled<ResourceStateGrabbable>(entity, false);
        if (trans.Value.y <= PlayVolumeFloor)
        {
            Ecb.SetComponentEnabled<Gravity>(entity, false);
            vel.Value.y = 0;
            
        }
    }
}
[BurstCompile]
public partial struct StackingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var playVolumeFloor = -SystemAPI.GetSingleton<Config>().PlayVolume.y;
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
    // TODO: Parallel schedule please. 
        new StackingJob
            {
                PlayVolumeFloor = playVolumeFloor, 
                Ecb = ecb
            }.Schedule();
    }
} 