using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(AttackingSystem))]
[UpdateBefore(typeof(BeeClampingSystem))]
partial struct KinematicMovementSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BeeConfig>();
    }

    public void OnDestroy(ref SystemState state) {
    }

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
