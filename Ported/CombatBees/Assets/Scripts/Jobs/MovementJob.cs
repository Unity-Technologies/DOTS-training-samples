using Unity.Entities;

partial struct MovementJob : IJobEntity {

    public float deltaTime;

    void Execute(ref PositionComponent positionComponent, in VelocityComponent velocityComponent) {
        positionComponent.Value += velocityComponent.Value * deltaTime;
    }
}