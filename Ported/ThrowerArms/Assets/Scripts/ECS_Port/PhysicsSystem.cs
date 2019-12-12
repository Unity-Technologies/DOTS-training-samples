using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PhysicsSystem : JobComponentSystem
{
    [BurstCompile]
    struct GravityJob : IJobForEach<Velocity, Gravity>
    {
        public float3 gravityTimesDeltaTime;
        public static readonly float3 kGravity = new float3(0.0f, -25.0f, 0.0f);

        public void Execute(ref Velocity velocity, [ReadOnly] ref Gravity gravity)
        {
            velocity.Value += gravityTimesDeltaTime;
        }
    }

    [BurstCompile]
    struct VelocityJob : IJobForEach<Translation, Velocity>
    {
        public float deltaTime;
        
        public void Execute(ref Translation translation, [ReadOnly] ref Velocity velocity)
        {
            translation.Value += velocity.Value * deltaTime;
        }
    }

    [BurstCompile]
    struct RotationJob : IJobForEach<Rotation, AngularVelocity>
    {
        public float deltaTime;

        public void Execute(ref Rotation rotation, [ReadOnly] ref AngularVelocity angularVelocity)
        {
            //quaternion combinedRotation = mul(rotation.Value, angularVelocity.rotation);
            //rotation.Value = math.slerp(rotation.Value, combinedRotation, deltaTime);
         //   rotation.Value = math.mul(quaternion.AxisAngle(angularVelocity.Value, math.length(angularVelocity.Value) * deltaTime), rotation.Value);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var gravityJob = new GravityJob();
        gravityJob.gravityTimesDeltaTime = GravityJob.kGravity * new float3(UnityEngine.Time.deltaTime);
        JobHandle gravityJobHandle = gravityJob.Schedule(this, inputDependencies);

        var velocityJob = new VelocityJob();
        velocityJob.deltaTime = UnityEngine.Time.deltaTime;
        JobHandle velocityJobHandle = velocityJob.Schedule(this, gravityJobHandle);

        RotationJob rotationJob = new RotationJob();
        rotationJob.deltaTime = UnityEngine.Time.deltaTime;
        JobHandle rotationJobHandle = rotationJob.Schedule(this, inputDependencies);

        return JobHandle.CombineDependencies(velocityJobHandle, rotationJobHandle);
    }
}