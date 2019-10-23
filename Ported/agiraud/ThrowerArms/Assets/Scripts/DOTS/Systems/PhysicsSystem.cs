using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
public class PhysicsSystem : JobComponentSystem
{
    [BurstCompile]
    struct AddPhysicsVelocityToObjectJob : IJobForEach<Translation, Rotation, Physics>
    {
        public float dt;
        public void Execute(ref Translation translation, ref Rotation rotation, [ReadOnly]ref Physics physics)
        {
            translation.Value += physics.velocity * dt;
            rotation.Value = math.mul(quaternion.AxisAngle(physics.angularVelocity, math.length(physics.angularVelocity) * dt), rotation.Value);
        }
    }

    [BurstCompile]
    struct AddGravitySystemJob : IJobForEach<Physics>
    {
        public float dt;
        public void Execute(ref Physics physics)
        {
            if (!physics.flying) return;
            physics.velocity.y -= physics.GravityStrength * dt;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var jobAddGravity = new AddGravitySystemJob() {  dt = Time.deltaTime };
        var jobUpdateObject = new AddPhysicsVelocityToObjectJob() {  dt = Time.deltaTime };

        var jhAddGravity = jobAddGravity.Schedule(this, inputDependencies);
        var jhUpdateObject = jobUpdateObject.Schedule(this, jhAddGravity);
        return jhUpdateObject;
    }
}