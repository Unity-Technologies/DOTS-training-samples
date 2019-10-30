using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;

namespace Unity.Rendering
{
    /// <summary>
    /// Copies the BoneIndexOffsets on the skinned entities to the index offsets material properties of the skinned mesh entities.
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateBefore(typeof(RenderMeshSystemV2))]
    public class CopySkinnedEntityDataToRenderEntity : JobComponentSystem
    {
        [BurstCompile]
        private struct IterateSkinnedEntityRefJob : IJobForEachWithEntity<SkinnedEntityReference, BoneIndexOffsetMaterialProperty>
        {
            [ReadOnly] public ComponentDataFromEntity<BoneIndexOffset> BoneIndexOffsets;

            public void Execute(
                Entity entity,
                int index,
                ref SkinnedEntityReference skinnedEntity,
                ref BoneIndexOffsetMaterialProperty boneIndexOffset)
            {
                boneIndexOffset = new BoneIndexOffsetMaterialProperty { Value = BoneIndexOffsets[skinnedEntity.Value].Value };
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new IterateSkinnedEntityRefJob
            {
                BoneIndexOffsets = GetComponentDataFromEntity<BoneIndexOffset>(true),
            };

            return job.Schedule(this, inputDeps);
        }
    }
}
