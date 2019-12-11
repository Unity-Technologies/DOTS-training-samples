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

        m_GrabbersQuery = GetEntityQuery(ComponentType.ReadOnly<Grabber>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        NativeArray<Entity> pickupArray = new NativeArray<Entity>(m_GrabbersQuery.CalculateEntityCount(), Allocator.TempJob, NativeArrayOptions.ClearMemory);

        JobHandle assignJobHandle = Entities.WithName("AssignRocksToArray")
            .ForEach((Entity e, ref Translation translation, ref Grabable grabable) =>
            {
                int index = (int)math.round(translation.Value.x);
                pickupArray[index] = e;
        }).Schedule(inputDependencies);

        EntityCommandBuffer.Concurrent concurrentBuffer = entityCommandBuffer.ToConcurrent();
        JobHandle grabJobHandle = Entities.WithName("GrabRocksFromArray")
            .WithDeallocateOnJobCompletion(pickupArray)
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref Grabber grabable) =>
        {
            int index = (int)math.round(translation.Value.x);
            Entity pickEntity = pickupArray[index];

            if (pickEntity != Entity.Null)
            {
                // Do something with the picked entity.


                // Remove the components so we do not pick another entity.
                concurrentBuffer.RemoveComponent<Grabber>(entityInQueryIndex, entity);
                concurrentBuffer.RemoveComponent<Grabable>(entityInQueryIndex, pickEntity);
            }
        }).Schedule(assignJobHandle);
        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(grabJobHandle);

        return grabJobHandle;
    }
}