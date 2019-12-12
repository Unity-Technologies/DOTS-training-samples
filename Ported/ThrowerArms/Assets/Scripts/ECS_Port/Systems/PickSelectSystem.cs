using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class PickSelectSystem : JobComponentSystem
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    private static int CalculateIndex(in Translation translation)
    {
        return (int)math.round(translation.Value.x / ArmSpawner.Spacing);
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        NativeArray<Entity> pickupArray = new NativeArray<Entity>(ArmSpawner.Count, Allocator.TempJob, NativeArrayOptions.ClearMemory);
        var count = ArmSpawner.Count;
        JobHandle assignJobHandle = Entities.WithName("AssignRocksToArray")
            .WithNativeDisableParallelForRestriction(pickupArray)
            .WithAll<GrabbableState>()
            .ForEach((Entity e, in Translation translation) =>
            {
                int index = CalculateIndex(translation);
                if (index < count)
                {
                    pickupArray[index] = e;
                }
        }).Schedule(inputDependencies);

        var accessor = GetComponentDataFromEntity<Scale>(isReadOnly: true);
        
        EntityCommandBuffer.Concurrent concurrentBuffer = entityCommandBuffer.ToConcurrent();
        JobHandle grabJobHandle = Entities.WithName("GrabRocksFromArray")
            .WithDeallocateOnJobCompletion(pickupArray)
            .WithReadOnly(pickupArray)
            .WithReadOnly(accessor)
            .WithAll<FindGrabbableTargetState>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
        {
            Entity pickEntity = pickupArray[CalculateIndex(translation)];

            if (pickEntity != Entity.Null)
            {
                // Remove the components so we do not pick another entity.
                concurrentBuffer.RemoveComponent<FindGrabbableTargetState>(entityInQueryIndex, entity);
                concurrentBuffer.RemoveComponent<GrabbableState>(entityInQueryIndex, pickEntity);
                
                // Add the reach for target state.
                concurrentBuffer.AddComponent(entityInQueryIndex, entity, new ReachForTargetState
                {
                    TargetEntity = pickEntity,
                    TargetSize = accessor[pickEntity].Value,
                    HandTarget = new float3(float.NaN, float.NaN, float.NaN) 
                });
            }
        }).Schedule(assignJobHandle);
        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(grabJobHandle);

        return grabJobHandle;
    }
}