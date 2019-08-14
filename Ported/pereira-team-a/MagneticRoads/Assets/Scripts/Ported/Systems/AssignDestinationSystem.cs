using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AssignDestinationSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem EntityCommandBufferSystem;


    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    struct AssignDestinationJob : IJobForEachWithEntity<FindTargetComponent, CurrentIntersectionIndexComponent>
    {
        [ReadOnly] public DynamicBuffer<IntersectionBuffer> intersectionBuffer;
        public float deltaTime;
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public unsafe void Execute(Entity entity, int index, [ReadOnly] ref FindTargetComponent findTarget,
            ref CurrentIntersectionIndexComponent currentIndex)
        {
            //1. Get the id from the currentIndex
            //2. Read the data from the DynamicBuffer with the id
            //3. GetNeighbors
            //4. Select a random neighbor using noise
            //5. Set MovementComponent
            //6. Remove the FindTargetComponent
            //7. Update currentIndex

            var intersection = intersectionBuffer[currentIndex.id];
            int targetNeighborId = (int) (noise.cnoise(new float2(deltaTime, currentIndex.id * 17)) * 3);
            var targetIntersectionId = intersection.neighbors[targetNeighborId];
            CommandBuffer.SetComponent(index, entity, new MovementComponent {targetPosition = intersectionBuffer[targetIntersectionId].position});
            CommandBuffer.RemoveComponent<FindTargetComponent>(index, entity);
            currentIndex.id = targetIntersectionId;
        }
    }

    protected override JobHandle OnUpdate(JobHandle handle)
    {
        var bufferEntity = GetSingletonEntity<IntersectionBuffer>();
        var buffer = EntityManager.GetBuffer<IntersectionBuffer>(bufferEntity);

        var job = new AssignDestinationJob
        {
            intersectionBuffer = buffer,
            deltaTime = Time.deltaTime,
            CommandBuffer = EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var jobHandle = job.Schedule(this, handle);

        EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        
        return jobHandle;
    }
}