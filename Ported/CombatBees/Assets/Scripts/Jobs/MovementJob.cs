using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
partial struct MovementJob : IJobEntity {

    public float deltaTime;

    void Execute(ref TransformAspect prs, in Velocity velocity) {
        prs.Position += velocity.Value * deltaTime;
    }
}