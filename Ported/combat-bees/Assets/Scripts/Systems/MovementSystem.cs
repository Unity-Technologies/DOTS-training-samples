using Unity.Burst;
using Unity.Collections;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementUtilities
{
    [BurstCompile]
    public static float3 ComputeTargetVelocity(in float3 currentPos, in float3 targetPos, in float gravityDt,
        in float timeStep, in float maxSpeed)
    {
        var newPos = targetPos;

        // Compute new velocity
        var targetVelocity = (newPos - currentPos) / timeStep;
        var desiredSpeed = math.length(targetVelocity);

        // make sure bee doesn't fly faster than it can
        var speed = math.min(desiredSpeed, maxSpeed);

        targetVelocity = math.select(float3.zero, (speed / desiredSpeed) * targetVelocity, speed > 0);

        targetVelocity.y -= gravityDt; // counter act gravity
        return targetVelocity;
    }
}

[BurstCompile]
public struct MovementJob : IJobChunk
{
    public ComponentTypeHandle<LocalToWorldTransform> TransformHandle;
    public ComponentTypeHandle<Velocity> VelocityHandle;

    [ReadOnly] public float TimeStep;
    [ReadOnly] public float GravityDt;
    [ReadOnly] public AABB FieldArea;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var chunkEntityEnumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.ChunkEntityCount);
        var transforms = chunk.GetNativeArray(TransformHandle);
        var velocities = chunk.GetNativeArray(VelocityHandle);

        while (chunkEntityEnumerator.NextEntityIndex(out var i))
        {
            var tmComp = transforms[i];
            var velComp = velocities[i];

            var velNew = velComp.Value;
            var pos = tmComp.Value.Position;
            float k_DeepImpactVel = 0.5f;
            float k_Restitution = 0.4f;
            float k_FrictionDamping = 0.3f;

            velNew.y += GravityDt;
            pos += TimeStep * velNew;
            bool onGround = pos.y < FieldArea.Min.y;

            if (onGround && velNew.y < -k_DeepImpactVel) // deep impact
            {
                velNew.y = -velNew.y * k_Restitution;
                velNew.xz *= k_FrictionDamping;
            }
            else
            {
                velNew = math.select(velNew,float3.zero, onGround);
            }

            // clamp to field
            pos = math.clamp(pos, FieldArea.Min, FieldArea.Max);

            tmComp.Value.Position = pos;
            velComp.Value = velNew;

            // derive heading of velocity
            var heading = math.atan2(velNew.x, velNew.z);
            var headingQ = quaternion.AxisAngle(math.up(), heading);

#if true
            // derive pitch of velocity, by bringing velocity into local space of object (with z forward) and
            // calculating the angle of the velocity in the local yz plane of the object
            var velForwardLocal = math.rotate(math.inverse(headingQ), velNew);
            var pitch = math.atan2(velForwardLocal.y, velForwardLocal.z);
            var pitchQ = quaternion.AxisAngle(math.left(), pitch);

            // allow objects to have pitch and heading
            var targetQ = math.mul(headingQ, pitchQ);
#else
            // allow objects to only have a heading 
            var targetQ = headingQ;
#endif
            // Interpolate between target rotation and current rotation based on velocity on xz plane.
            // Objects with close to zero velocity on xz plane will maintain their current orientation
            var k_minVelForRotChangeSq = 0.001f;
            var k_decayFactor = 2.0f;

            // alpha drops to zero quickly with rising velocity and for a velocity close to zero, i.e., 
            // smaller than sqrt(k_minVelForRotChangeSq), alpha is 1.
            // The decay with increasing velocity can be controlled with k_decayFactor.
            var alpha = math.min(1.0f,
                -math.tanh((math.lengthsq(velNew.xz) - k_minVelForRotChangeSq) * k_decayFactor) + 1.0f);
            var currentQ = tmComp.Value.Rotation;
            // alpha = 0: target orientation, alpha = 1: current orientation
            var newQ = math.slerp(targetQ, currentQ, alpha);

            tmComp.Value.Rotation = newQ;

            transforms[i] = tmComp;
            velocities[i] = velComp;
        }
    }
}

[BurstCompile]
partial struct MovementSystem : ISystem
{
    private EntityQuery Query;
    public ComponentTypeHandle<LocalToWorldTransform> TransformHandle;
    public ComponentTypeHandle<Velocity> VelocityHandle;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Query = SystemAPI.QueryBuilder().WithAllRW<LocalToWorldTransform, Velocity>().Build();
        TransformHandle = state.GetComponentTypeHandle<LocalToWorldTransform>();
        VelocityHandle = state.GetComponentTypeHandle<Velocity>();
        
        state.RequireForUpdate<BeeConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BeeConfig>();

        TransformHandle.Update(ref state);
        VelocityHandle.Update(ref state);
        var job = new MovementJob()
        {
            TransformHandle = TransformHandle,
            VelocityHandle = VelocityHandle,
            TimeStep = state.Time.DeltaTime,
            GravityDt = config.gravity * state.Time.DeltaTime,
            FieldArea = config.fieldArea
        };

        state.Dependency = job.ScheduleParallel(Query, state.Dependency);
    }
}