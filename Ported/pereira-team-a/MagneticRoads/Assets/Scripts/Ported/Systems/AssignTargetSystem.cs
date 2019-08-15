using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
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
        [ReadOnly] public DynamicBuffer<Spline> splineBuffer;
        public float deltaTime;
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public unsafe void Execute(Entity entity, int index, [ReadOnly] ref FindTarget findTarget,
            ref TargetIntersectionIndex targetIndex)
        {
            //1.Get the TargetPosition from the targetIndex

            //2.Read the data from the DynamicBuffer with the TargetPosition
            //3.GetNeighbors
            //4.Select a random neighbor using noise
            //5.Set SplineData
            //6.Remove the FindTarget
            //7.Update targetIndex

            var intersection = intersectionBuffer[targetIndex.Value];
            //var targetNeighborId = (int)(noise.cnoise(new float2(deltaTime, targetIndex.Value * 17)) * intersection.SplineIdCount);
            var targetNeighborId = (int) (((math.sin(deltaTime) + 1) * 0.5f) * (intersection.SplineIdCount));
            
            var targetSplineId = intersection.SplineId0; // 0
            if(targetNeighborId == 1)
                targetSplineId = intersection.SplineId1; // 1
            if(targetNeighborId == 2)
                targetSplineId = intersection.SplineId2; // 2

            var spline = splineBuffer[targetSplineId];

            var splineData = new SplineData
            {
                StartPosition = intersection.Position,
                TargetPosition = intersectionBuffer[spline.EndIntersectionId].Position,
                spline = spline
            };

            CommandBuffer.SetComponent(index, entity, splineData);
            CommandBuffer.RemoveComponent<FindTarget>(index, entity);
            targetIndex.Value = spline.EndIntersectionId;
        }
    }

    protected override JobHandle OnUpdate(JobHandle handle)
    {
        var intersectionBuffer = EntityManager.GetBuffer<IntersectionPoint>(GetSingletonEntity<IntersectionPoint>());
        var splineBuffer = EntityManager.GetBuffer<Spline>(GetSingletonEntity<Spline>());

        var job = new AssignDestinationJob
        {
            intersectionBuffer = intersectionBuffer,
            splineBuffer = splineBuffer,
            deltaTime = Time.time,
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var jobHandle = job.Schedule(this, handle);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}