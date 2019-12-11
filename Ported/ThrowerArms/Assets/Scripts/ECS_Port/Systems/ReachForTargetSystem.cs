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
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref ReachForTargetState reachState, ref ArmComponent arm) =>
        {
            float3 desiredRockTranslation = translationFromEntityAccessor[reachState.TargetEntity].Value;
            float3 delta = desiredRockTranslation - translation.Value;

            bool reachedRockSuccessfully = math.lengthsq(delta) < math.lengthsq(ArmConstants.MaxReach);
            
            if (reachedRockSuccessfully)
            {
                if (math.all(reachState.FullyReachedOutHandTranslation == new float3(float.NaN, float.NaN, float.NaN)))
                {
                    float3 flatDelta = delta;
                    flatDelta.y = 0;
                
                    flatDelta = math.normalize(flatDelta);

                    reachState.FullyReachedOutHandTranslation =
                        desiredRockTranslation + new float3(0, 1, 0) * reachState.TargetSize * 0.5f - flatDelta * reachState.TargetSize * 0.5f;
                }
                
                reachState.ReachTimer += k_deltaTime / ArmConstants.ReachDuration;

                if (reachState.ReachTimer < 1f)
                {
                    return;
                }
                
                ecb.AddComponent(reachState.TargetEntity, new HeldState());
                
                ecb.RemoveComponent<ReachForTargetState>(entity);
                
                ecb.AddComponent(entity, new HoldTargetState
                {
                    CountdownToStartWindingUp = new Random().NextFloat(-1f, 0f),
                    HeldTargetOffsetFromHand = math.mul(math.inverse(arm.HandMatrix), new float4(desiredRockTranslation, 1)).xyz
                });
            }
            else
            {
                ecb.AddComponent(reachState.TargetEntity, new GrabbableState());
                
                ecb.RemoveComponent<ReachForTargetState>(entity);
                
                ecb.AddComponent(entity, new IdleState());
                ecb.AddComponent(entity, new FindGrabbableTargetState());
            }
        }).Schedule(inputDeps);
    }
}