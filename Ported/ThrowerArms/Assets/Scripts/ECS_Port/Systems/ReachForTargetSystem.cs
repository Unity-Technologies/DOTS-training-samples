using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ReachForTargetSystem : JobComponentSystem
    {
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
                    float3 flatDelta = delta;
                    flatDelta.y = 0;
                    
                    float3 normalisedFlatDelta = math.normalize(flatDelta);
                    
                    
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