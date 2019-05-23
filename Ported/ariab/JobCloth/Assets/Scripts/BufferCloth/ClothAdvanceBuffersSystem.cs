using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(ClothConstraintSolverSystem))]
public class ClothCopyProjectedToPreviousSystem : JobComponentSystem
{
    [BurstCompile]
    // todo: Would be better if I could alias these buffers each frame rather than perform copies
    // i.e. "projected" would be next frame's "current", 
    unsafe struct ClothAdvanceBuffersJob : IJobForEach_BBB<ClothProjectedPosition, ClothCurrentPosition, ClothPreviousPosition>
    {
        public void Execute(DynamicBuffer<ClothProjectedPosition> projected, DynamicBuffer<ClothCurrentPosition> current, DynamicBuffer<ClothPreviousPosition> previous)
        {
            var copySize = sizeof(float3) * previous.Length;
            
            // Copy current to previous
            {
                var srcPtr = current.GetUnsafePtr();
                var dstPtr = previous.GetUnsafePtr();

                UnsafeUtility.MemCpy(dstPtr, srcPtr, copySize);
            }
            
            // Copy projected to current
            {
                var srcPtr = projected.GetUnsafePtr();
                var dstPtr = current.GetUnsafePtr();

                UnsafeUtility.MemCpy(dstPtr, srcPtr, copySize);
            }
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new ClothAdvanceBuffersJob();
        return job.Schedule(this, inputDependencies);
    }
}