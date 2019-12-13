using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class FindThrowTargetSystem : JobComponentSystem
{
    private EntityQuery m_ThrowTargetsQuery;

    private static int CalculateIndex(in Translation translation, float spacing)
    {
        return (int)math.round(translation.Value.x / spacing);
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        float spacing = ArmSpawner.Spacing;
        int count = ArmSpawner.Count;

        EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        NativeArray<Entity> throwTargetsArray = new NativeArray<Entity>(count, Allocator.TempJob, NativeArrayOptions.ClearMemory);

        JobHandle assignJobHandle = Entities.WithName("AssignTargetsToArray")
            .WithNativeDisableParallelForRestriction(throwTargetsArray)
            .WithAll<ThrowTargetState>()
            .ForEach((Entity e, in Translation translation) =>
            {
                int index = CalculateIndex(translation, spacing);
                if (index < count && index >= 0)
                {
                    throwTargetsArray[index] = e;
                }
            }).Schedule(inputDependencies);

        EntityCommandBuffer.Concurrent concurrentBuffer = entityCommandBuffer.ToConcurrent();
        JobHandle selectTargetJobHandle = Entities.WithName("AssignThrowTargetsToThrowers")
            .WithDeallocateOnJobCompletion(throwTargetsArray)
            .WithReadOnly(throwTargetsArray)
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation, in LookForThrowTargetState thrower) =>
            {
                int index = CalculateIndex(translation, spacing);
                Entity targetEntity = throwTargetsArray[index];

                if (targetEntity != Entity.Null)
                {
                    // Remove the components so we do not pick another entity.
                    concurrentBuffer.RemoveComponent<LookForThrowTargetState>(entityInQueryIndex, entity);
                    concurrentBuffer.RemoveComponent<ThrowTargetState>(entityInQueryIndex, targetEntity);

                    // Add the next state.
                    concurrentBuffer.AddComponent(entityInQueryIndex, entity, new WindingUpState
                    {
                        WindupTimer = -0.5f,
                        AimedTargetEntity = targetEntity,
                        HeldEntity = thrower.GrabbedEntity,
                    });
                }
            }).Schedule(assignJobHandle);
        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(selectTargetJobHandle);

        return selectTargetJobHandle;
    }
}