using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
[UpdateAfter(typeof(ThrowSystem))]
public class DestroySystem : JobComponentSystem
{
    EntityQuery m_group;

    [BurstCompile]
    struct DestroySystemJob : IJobForEachWithEntity<Translation, ResetPosition>
    {
        public float3 boundsMin;
        public float3 boundsMax;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, ref ResetPosition resetPos)
        {
            if (translation.Value.x < boundsMin.x || translation.Value.y < boundsMin.y || translation.Value.z < boundsMin.z ||
                translation.Value.x > boundsMax.x || translation.Value.y > boundsMax.y || translation.Value.z > boundsMax.z)
            {
                resetPos.needReset = true;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new DestroySystemJob();

        // TODO: extract and update based on the number of arms, etc
        job.boundsMin = ConstantManager.DestroyBoxMin;
        job.boundsMax = ConstantManager.DestroyBoxMax;

        var jobHandle = job.Schedule(m_group, inputDeps);
        return jobHandle;
    }

    protected override void OnCreate()
    {
        m_group = GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadWrite<ResetPosition>());
    }
}
