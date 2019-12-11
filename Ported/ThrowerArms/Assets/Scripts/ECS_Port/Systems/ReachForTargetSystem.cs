using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ReachForTargetSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var translationFromEntityAccessor = GetComponentDataFromEntity<Translation>();
            
            return Entities.ForEach((ref Translation translation, ref ReachForTargetState reachState, ref ArmComponent arm) =>
            {
                float3 desiredRockTranslation = translationFromEntityAccessor[reachState.TargetEntity].Value;
                float3 delta = desiredRockTranslation - translation.Value;

                if (math.lengthsq(delta) < math.pow(ArmConstants.MaxReach, 2))
                {
                    float3 flatDelta = delta;
                    flatDelta.y = 0;
                    
                    float3 normalisedFlatDelta = math.normalize(flatDelta);
                    
                    
                }
            }).Schedule(inputDeps);
        }
    }