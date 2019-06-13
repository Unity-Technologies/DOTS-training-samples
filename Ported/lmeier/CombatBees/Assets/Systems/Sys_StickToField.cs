using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(LateSimulationSystemGroup))][UpdateBefore(typeof(Sys_TranslationAddVelocity))]
public class Sys_StickToField : JobComponentSystem
{

    [BurstCompile]
    struct Sys_StickToFieldJob : IJobForEach<Translation, C_Velocity, Rotation>
    {
        [ReadOnly] public float dt;
        [ReadOnly] public float3 fieldBounds;
        [ReadOnly] public float gravity;

        public void Execute(ref Translation translation, ref C_Velocity velocity, ref Rotation rot)
        {

            if (System.Math.Abs(translation.Value.x) > fieldBounds.x * .5f)
            {
                translation.Value.x = (fieldBounds.x * .50001f) * sign(translation.Value.x);
                velocity.Value = float3(0);
                rot.Value = Unity.Mathematics.quaternion.LookRotation(float3(1,0,0), math.up());
            }
            else if (System.Math.Abs(translation.Value.z) > fieldBounds.z * .5f)
            {
                translation.Value.z = (fieldBounds.z * .50001f) * sign(translation.Value.z);
                velocity.Value = float3(0);
                rot.Value = Unity.Mathematics.quaternion.LookRotation(float3(0,0,1), math.up());
            }
            else if (System.Math.Abs(translation.Value.y) > fieldBounds.y * .5f)
            {
                translation.Value.y = (fieldBounds.y * .50001f) * sign(translation.Value.y);
                velocity.Value = float3(0);
                rot.Value = Unity.Mathematics.quaternion.LookRotation(float3(0,1,0), float3(1,0,0));
            }
            else
            {
                velocity.Value.y += gravity * dt;
            }

        }
    }

    private EntityQuery m_group;

    protected override void OnCreate()
    {
        m_group = GetEntityQuery(typeof(Translation), typeof(C_Velocity), typeof(Rotation), ComponentType.ReadOnly<Tag_Sticky>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        m_group.SetFilterChanged(typeof(Translation));
        var job = new Sys_StickToFieldJob()
        {
            dt = UnityEngine.Time.deltaTime,
            fieldBounds = Field.size,
            gravity = GameConstants.S.Gravity
        };

        return job.Schedule(m_group, inputDependencies);
    }
}