using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AssignTargetSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    struct AssignDestinationJob : IJobForEachWithEntity<FindTarget, TargetIntersectionIndex>
    {
        [ReadOnly] public DynamicBuffer<IntersectionPoint> intersectionBuffer;
        public float deltaTime;
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public unsafe void Execute(Entity entity, int index, [ReadOnly] ref FindTarget findTarget,
            ref TargetIntersectionIndex targetIndex)
        {
            //1. Get the Value from the targetIndex
            //2. Read the data from the DynamicBuffer with the Value
            //3. GetNeighbors
            //4. Select a random neighbor using noise
            //5. Set TargetPosition
            //6. Remove the FindTarget
            //7. Update targetIndex

            var intersection = intersectionBuffer[targetIndex.Value];
            int targetNeighborId = (int) (noise.cnoise(new float2(deltaTime, targetIndex.Value * 17)) * 3);
            var targetIntersectionId = intersection.Neighbors[targetNeighborId];
            CommandBuffer.SetComponent(index, entity, new TargetPosition {Value = intersectionBuffer[targetIntersectionId].Position});
            CommandBuffer.RemoveComponent<FindTarget>(index, entity);
            targetIndex.Value = targetIntersectionId;
        }
    }

    protected override JobHandle OnUpdate(JobHandle handle)
    {
        var bufferEntity = GetSingletonEntity<IntersectionPoint>();
        var buffer = EntityManager.GetBuffer<IntersectionPoint>(bufferEntity);

        var job = new AssignDestinationJob
        {
            intersectionBuffer = buffer,
            deltaTime = Time.deltaTime,
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var jobHandle = job.Schedule(this, handle);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        
        return jobHandle;
    }
}