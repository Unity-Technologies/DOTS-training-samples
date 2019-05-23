using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace JumpTheGun
{
    // For blocks marked with UpdateBlockTransformTag, use their position
    // and height to compute a new LocalToWorld matrix.
    [UpdateAfter(typeof(TerrainSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class BlockTransformSystem : JobComponentSystem
    {
        //[BurstCompile] // Can't currently add/remove components in Burst jobs
        struct BlockTransformJob : IJobForEachWithEntity<BlockPositionXZ, BlockHeight, UpdateBlockTransformTag, LocalToWorld>
        {
            public EntityCommandBuffer.Concurrent CommandBuffer;

            public void Execute(Entity entity, int index, [ReadOnly] ref BlockPositionXZ position,
                [ReadOnly] ref BlockHeight height, [ReadOnly] ref UpdateBlockTransformTag _, ref LocalToWorld localToWorld)
            {
                float3 fullPos = new float3(position.Value.x, 0.0f, position.Value.y);
                float3 fullScale = new float3(1.0f, height.Value, 1.0f);
                // TODO(cort): Use WriteGroups here instead
                localToWorld = new LocalToWorld
                {
                    Value = math.mul(float4x4.Translate(fullPos), float4x4.Scale(fullScale))
                };
                CommandBuffer.RemoveComponent<UpdateBlockTransformTag>(index, entity);
            }
        }

        private BeginSimulationEntityCommandBufferSystem _barrier;
        protected override void OnCreateManager()
        {
            _barrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var handle = new BlockTransformJob
            {
                CommandBuffer = _barrier.CreateCommandBuffer().ToConcurrent(),
            }.Schedule(this, inputDeps);
            _barrier.AddJobHandleForProducer(handle);
            return handle;
        }
    }
}
