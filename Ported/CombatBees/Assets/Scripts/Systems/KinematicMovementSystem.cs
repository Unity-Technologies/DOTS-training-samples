using Unity.Entities;
using Unity.Mathematics;

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
            random = Random.CreateFromIndex((uint)state.Time.ElapsedTime * 1000),
            deltaTime = state.Time.DeltaTime,
            jitter = beeConfig.flightJitter,
            damping = beeConfig.damping
        }.Schedule(state.Dependency);
    }
}
