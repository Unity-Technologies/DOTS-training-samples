using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ReachForTargetSystem : JobComponentSystem
{   
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var endSimulationEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer ecb = endSimulationEcbSystem.CreateCommandBuffer();
        EntityCommandBuffer.Concurrent concurrentBuffer = ecb.ToConcurrent();

        var handMatrixAccessor = GetComponentDataFromEntity<HandMatrix>(true);
        var translationFromEntityAccessor = GetComponentDataFromEntity<Translation>(true);
        
        JobHandle reachForRockJob = Entities.WithName("TryReachForTargetRock")
            .WithReadOnly(translationFromEntityAccessor)
            .WithReadOnly(handMatrixAccessor)
            .ForEach((Entity entity, int entityInQueryIndex, ref ReachForTargetState reachingState, ref ArmComponent arm, in Translation translation) =>
        {
            float3 desiredRockTranslation = translationFromEntityAccessor[reachingState.TargetEntity].Value;
            float3 delta = desiredRockTranslation - translation.Value;

            bool rockIsReachable = math.lengthsq(delta) < math.lengthsq(ArmConstants.MaxReach);
            if (!rockIsReachable)
            {
                concurrentBuffer.AddComponent(entityInQueryIndex, reachingState.TargetEntity, new GrabbableState());
                concurrentBuffer.AddComponent(entityInQueryIndex, entity, new IdleState());
                concurrentBuffer.AddComponent(entityInQueryIndex, entity, new FindGrabbableTargetState());
                concurrentBuffer.RemoveComponent<ReachForTargetState>(entityInQueryIndex, entity);
                return;
            }

            float3 flatDelta = delta;
            flatDelta.y = 0;
            
            flatDelta = math.normalize(flatDelta);

            reachingState.HandTarget =
                desiredRockTranslation + new float3(0, 1, 0) * reachingState.TargetSize * 0.5f - flatDelta * reachingState.TargetSize * 0.5f;
            arm.HandTarget = reachingState.HandTarget;
            
            if (arm.ReachTimer < 1f)
            {
                return;
            }

            arm.ThrowTimer = 0.0f;
            concurrentBuffer.RemoveComponent<ReachForTargetState>(entityInQueryIndex, entity);

            concurrentBuffer.AddComponent(entityInQueryIndex, entity, new LookForThrowTargetState
            {
                GrabbedEntity = reachingState.TargetEntity,
                TargetSize = translationFromEntityAccessor[reachingState.TargetEntity].Value
            });
            concurrentBuffer.AddComponent(entityInQueryIndex, reachingState.TargetEntity, new GrabbedState
            {
                GrabbingEntity = entity,
                localPosition = math.transform(math.inverse(handMatrixAccessor[entity].Value), desiredRockTranslation)
            });
            concurrentBuffer.AddComponent<HoldingRockState>(entityInQueryIndex, entity);
        }).Schedule(inputDeps);
        endSimulationEcbSystem.AddJobHandleForProducer(reachForRockJob);

        return reachForRockJob;
    }
}