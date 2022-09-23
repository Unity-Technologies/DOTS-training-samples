using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateAfter(typeof(AttackingSystem))]
[UpdateBefore(typeof(BeeClampingSystem))]
partial struct KinematicMovementSystem : ISystem {
    
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BeeConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var beeConfig = SystemAPI.GetSingleton<BeeConfig>();

        state.Dependency = new MovementJob() {
            deltaTime = state.Time.DeltaTime,
        }.ScheduleParallel(state.Dependency);

        state.Dependency = new JitterJob()
        {
            randomBase = (int) (state.Time.ElapsedTime * 10000),
            deltaTime = state.Time.DeltaTime,
            jitter = beeConfig.flightJitter,
            damping = beeConfig.damping
        }.ScheduleParallel(state.Dependency);
    }
}
