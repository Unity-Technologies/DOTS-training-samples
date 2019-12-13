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

    private static int CalculateIndex(in Translation translation, float spacing)
    {
        return (int)math.round((translation.Value.x+1f) / spacing);
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        float spacing = ArmSpawner.Spacing;
        EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        NativeArray<Entity> pickupArray = new NativeArray<Entity>(ArmSpawner.Count, Allocator.TempJob, NativeArrayOptions.ClearMemory);
        var count = ArmSpawner.Count;
        JobHandle assignJobHandle = Entities.WithName("AssignRocksToArray")
            .WithNativeDisableParallelForRestriction(pickupArray)
            .WithAll<GrabbableState>()
            .ForEach((Entity e, in Translation translation) =>
            {
                int index = CalculateIndex(translation, spacing);
                if (index < count && index >= 0)
                {
                    pickupArray[index] = e;
                }
        }).Schedule(inputDependencies);

        var translationFromEntityAccessor = GetComponentDataFromEntity<Translation>(true);
        assignJobHandle.Complete();

        EntityCommandBuffer.Concurrent concurrentBuffer = entityCommandBuffer.ToConcurrent();
        JobHandle grabJobHandle = Entities.WithName("GrabRocksFromArray")
            .WithDeallocateOnJobCompletion(pickupArray)
            .WithReadOnly(pickupArray)
            .WithReadOnly(translationFromEntityAccessor)
            .WithAll<FindGrabbableTargetState>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation, in ArmComponent arm) =>
        {
            Entity pickEntity = pickupArray[CalculateIndex(translation, spacing)-1];
            if (pickEntity == Entity.Null)
                return;

            float3 rockTranslation = translationFromEntityAccessor[pickEntity].Value;
            float3 delta = rockTranslation - translation.Value;

            bool rockIsReachable = math.lengthsq(delta) < math.lengthsq(ArmConstants.MaxReach);
            if (!rockIsReachable)
            {
                // Remove the components so we do not pick another entity.
                concurrentBuffer.RemoveComponent<FindGrabbableTargetState>(entityInQueryIndex, entity);
                concurrentBuffer.RemoveComponent<GrabbableState>(entityInQueryIndex, pickEntity);
                
                // Add the reach for target state.
                concurrentBuffer.AddComponent(entityInQueryIndex, entity, new ReachForTargetState
                {
                    TargetEntity = pickEntity,
                });
                concurrentBuffer.RemoveComponent<IdleState>(entityInQueryIndex, entity);
            }
        }).Schedule(assignJobHandle);
        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(grabJobHandle);

        return grabJobHandle;
    }
}