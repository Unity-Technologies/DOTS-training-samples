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
        NativeArray<Entity> throwTargetsArray = new NativeArray<Entity>(ArmSpawner.Count, Allocator.TempJob, NativeArrayOptions.ClearMemory);

        JobHandle assignJobHandle = Entities.WithName("AssignTargetsToArray")
            .WithNativeDisableParallelForRestriction(throwTargetsArray)
            .ForEach((Entity e, in Translation translation, in ThrowTargetState throwTarget) =>
            {
                int index = CalculateIndex(translation);
                if (index < ArmSpawner.Count)
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
                int index = CalculateIndex(translation);
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