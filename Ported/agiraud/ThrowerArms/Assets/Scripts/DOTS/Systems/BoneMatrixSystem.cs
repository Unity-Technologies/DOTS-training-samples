using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
[UpdateAfter(typeof(SpawnerSystem))]
public class BoneMatrixSystem : JobComponentSystem
{
    [BurstCompile]
    struct BoneMatrixSystemJob : IJobForEachWithEntity<BoneData, LocalToWorld>
    {
        [ReadOnly] public BufferFromEntity<BoneJoint> BoneChain;
        [ReadOnly] public ComponentDataFromEntity<HandAxis> Up;
        
        public void Execute(Entity entity, int jobIndex, [ReadOnly] ref BoneData boneData, ref LocalToWorld transform)
        {
            var chain = BoneChain[boneData.Parent];
            var delta = chain[boneData.ChainIndex + 1].JointPos - chain[boneData.ChainIndex].JointPos;

            var trs = float4x4.TRS(chain[boneData.ChainIndex].JointPos + delta * .5f,
                quaternion.LookRotation(delta, Up[boneData.Parent].Up),
                new float3(boneData.Thickness, boneData.Thickness, math.length(delta)));

            transform = new LocalToWorld { Value = trs };
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new BoneMatrixSystemJob
        {
            BoneChain = GetBufferFromEntity<BoneJoint>(true),
            Up = GetComponentDataFromEntity<HandAxis>(true)
        };
        return job.Schedule(this, inputDeps);
    }
}
