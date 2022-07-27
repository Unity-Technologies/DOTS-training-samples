using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[WithAll(typeof(Gravity), typeof(Resource))]
[BurstCompile]
partial struct StackingJob : IJobEntity
{
    public float PlayVolumeFloor;
    public EntityCommandBuffer Ecb;
    void Execute(in Entity entity, ref Velocity vel, ref Translation trans)
    {
        Ecb.SetComponentEnabled<ResourceStateGrabbable>(entity, false);
        if (trans.Value.y <= PlayVolumeFloor+1)
        {
            Ecb.SetComponentEnabled<Gravity>(entity, false);
            vel.Value.y = 0;
            // trans.Value.y += 1;

        }
    }
}
[BurstCompile]
public partial struct StackingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
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