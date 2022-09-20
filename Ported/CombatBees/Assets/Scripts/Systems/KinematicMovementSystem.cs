using Unity.Entities;

partial struct KinematicMovementSystem : ISystem {
    public void OnCreate(ref SystemState state) {
    }

    public void OnDestroy(ref SystemState state) {
    }

    public void OnUpdate(ref SystemState state) {
        new MovementJob() {
            deltaTime = state.Time.DeltaTime
        }.ScheduleParallel();
    }
}