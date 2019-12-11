using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ReachForTargetSystem : JobComponentSystem
{
    private const float k_deltaTime = 1 / 50f;
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var endSimulationEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer ecb = endSimulationEcbSystem.CreateCommandBuffer();
        
        var translationFromEntityAccessor = GetComponentDataFromEntity<Translation>();
        
        return Entities.WithName("TryReachForTargetRock")
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref ReachForTargetState reachingState, ref ArmComponent arm) =>
        {
            float3 desiredRockTranslation = translationFromEntityAccessor[reachingState.TargetEntity].Value;
            float3 delta = desiredRockTranslation - translation.Value;

            bool reachedRockSuccessfully = math.lengthsq(delta) < math.lengthsq(ArmConstants.MaxReach);
            
            if (reachedRockSuccessfully)
            {
                if (math.all(reachingState.FullyReachedOutHandTranslation == new float3(float.NaN, float.NaN, float.NaN)))
                {
                    float3 flatDelta = delta;
                    flatDelta.y = 0;
                
                    flatDelta = math.normalize(flatDelta);

                    reachingState.FullyReachedOutHandTranslation =
                        desiredRockTranslation + new float3(0, 1, 0) * reachingState.TargetSize * 0.5f - flatDelta * reachingState.TargetSize * 0.5f;
                }
                
                reachingState.ReachTimer += k_deltaTime / ArmConstants.ReachDuration;

                if (reachingState.ReachTimer < 1f)
                {
                    return;
                }
                
                ecb.AddComponent(reachingState.TargetEntity, new GrabbedState());
                
                ecb.RemoveComponent<ReachForTargetState>(entity);
                
                ecb.AddComponent(entity, new LookForThrowTargetState
                {
                    GrabbedEntity = reachingState.TargetEntity
                });
                ecb.AddComponent(entity, new GrabbingState
                {
                    GrabbedEntity = reachingState.TargetEntity
                });
            }
            else
            {
                ecb.AddComponent(reachingState.TargetEntity, new GrabbableState());
                
                ecb.RemoveComponent<ReachForTargetState>(entity);
                
                ecb.AddComponent(entity, new IdleState());
                ecb.AddComponent(entity, new FindGrabbableTargetState());
            }
        }).Schedule(inputDeps);
    }
}