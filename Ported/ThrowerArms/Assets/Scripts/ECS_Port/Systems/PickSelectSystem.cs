using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class PickSelectSystem : JobComponentSystem
{
    private EntityQuery m_GrabbersQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_GrabbersQuery = GetEntityQuery(ComponentType.ReadOnly<FindGrabbableTargetState>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        NativeArray<Entity> pickupArray = new NativeArray<Entity>(m_GrabbersQuery.CalculateEntityCount(), Allocator.TempJob, NativeArrayOptions.ClearMemory);

        JobHandle assignJobHandle = Entities.WithName("AssignRocksToArray")
            .ForEach((Entity e, ref Translation translation, ref GrabbableState grabbable) =>
            {
                int index = (int)math.round(translation.Value.x);
                pickupArray[index] = e;
        }).Schedule(inputDependencies);

        var accessor = GetComponentDataFromEntity<Scale>(isReadOnly: true);
        
        EntityCommandBuffer.Concurrent concurrentBuffer = entityCommandBuffer.ToConcurrent();
        JobHandle grabJobHandle = Entities.WithName("GrabRocksFromArray")
            .WithDeallocateOnJobCompletion(pickupArray)
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref FindGrabbableTargetState grabber) =>
        {
            int index = (int)math.round(translation.Value.x);
            Entity pickEntity = pickupArray[index];

            if (pickEntity != Entity.Null)
            {
                // Do something with the picked entity.

                // Remove the components so we do not pick another entity.
                concurrentBuffer.RemoveComponent<FindGrabbableTargetState>(entityInQueryIndex, entity);
                concurrentBuffer.RemoveComponent<GrabbableState>(entityInQueryIndex, pickEntity);
                
                concurrentBuffer.AddComponent(entityInQueryIndex, entity, new ReachForTargetState
                {
                    ReachTimer = 1f,
                    TargetEntity = pickEntity,
                    TargetSize = accessor[pickEntity].Value
                });
            }
        }).Schedule(assignJobHandle);
        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(grabJobHandle);

        return grabJobHandle;
    }
}