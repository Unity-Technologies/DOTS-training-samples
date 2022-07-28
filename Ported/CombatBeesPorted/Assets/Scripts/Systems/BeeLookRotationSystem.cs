using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[WithAny(typeof(BlueTeam), typeof(YellowTeam))]
[BurstCompile]
partial struct BeeLookRotationJob : IJobEntity
{
    void Execute(in Velocity vel, ref Rotation rot)
    {
        rot.Value = quaternion.LookRotation(vel.Value, new float3(0f, 1f, 0f));
    }
}

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateBefore(typeof(BeeKillerSystem))]
[BurstCompile]
public partial struct BeeLookRotationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new BeeLookRotationJob().ScheduleParallel();
    }
}