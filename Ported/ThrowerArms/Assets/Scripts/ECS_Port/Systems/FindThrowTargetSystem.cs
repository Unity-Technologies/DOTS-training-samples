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

        m_ThrowTargetsQuery = GetEntityQuery(ComponentType.ReadOnly<ThrowTargetState>());
    }


    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        NativeArray<Entity> throwTargetsArray = new NativeArray<Entity>(m_ThrowTargetsQuery.CalculateEntityCount(), Allocator.TempJob, NativeArrayOptions.ClearMemory);

        JobHandle assignJobHandle = Entities.WithName("AssignTargetsToArray")
            .ForEach((Entity e, ref Translation translation, ref ThrowTargetState throwTarget) =>
            {
                int index = (int)math.round(translation.Value.x);
                throwTargetsArray[index] = e;
            }).Schedule(inputDependencies);

        EntityCommandBuffer.Concurrent concurrentBuffer = entityCommandBuffer.ToConcurrent();
        JobHandle selectTargetJobHandle = Entities.WithName("AssignThrowTargetsToThrowers")
            .WithDeallocateOnJobCompletion(throwTargetsArray)
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref LookForThrowTargetState thrower) =>
            {
                int index = (int)math.round(translation.Value.x);
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