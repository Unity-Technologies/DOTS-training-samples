using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ReachForTargetSystem : JobComponentSystem
{   
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        var endSimulationEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer ecb = endSimulationEcbSystem.CreateCommandBuffer();
        EntityCommandBuffer.Concurrent concurrentBuffer = ecb.ToConcurrent();

        var translationFromEntityAccessor = GetComponentDataFromEntity<Translation>();
        
        JobHandle reachForRockJob = Entities.WithName("TryReachForTargetRock")
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref ReachForTargetState reachingState, ref ArmComponent arm) =>
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
            reachingState.ReachTimer += deltaTime / ArmConstants.ReachDuration;

            if (reachingState.ReachTimer < 1f)
            {
                return;
            }

            concurrentBuffer.RemoveComponent<ReachForTargetState>(entityInQueryIndex, entity);

            concurrentBuffer.AddComponent(entityInQueryIndex, entity, new LookForThrowTargetState
            {
                GrabbedEntity = reachingState.TargetEntity
            });
            concurrentBuffer.AddComponent(entityInQueryIndex, reachingState.TargetEntity, new GrabbedState
            {
                GrabbingEntity = entity
            });
            concurrentBuffer.AddComponent<GrabbingState>(entityInQueryIndex, entity);
        }).Schedule(inputDeps);
        endSimulationEcbSystem.AddJobHandleForProducer(reachForRockJob);

        return reachForRockJob;
    }
}