using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace src
{
    public class TranslationFollowingSystem : JobComponentSystem
    {
        const float k_SpeedIncrease = 0.1f;
        const float k_SpeedDecrease = 0.2f;
        const float k_DistanceEpsilon = 0.0005f;
        private const float k_MaxRelativeSpeedDifference = 0.5f;
        private EntityQuery m_EntityQuery;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var translationAccessor = GetComponentDataFromEntity<Translation>(true);
            var speedAccessor = GetComponentDataFromEntity<Speed>(true);

            var speedArray = new NativeArray<Speed>(m_EntityQuery.CalculateEntityCountWithoutFiltering(), Allocator.TempJob);
            
            var jobHandle = Entities
                .WithReadOnly(translationAccessor)
                .WithReadOnly(speedAccessor)
                .WithStoreEntityQueryInField(ref m_EntityQuery)
                .ForEach((int entityInQueryIndex, Speed speed, in FollowTranslation followTranslation, in Translation translation) =>
                {
                    var targetTranslation = translationAccessor[followTranslation.Target];
                    var speedOfTarget = speedAccessor[followTranslation.Target];
                    
                    var distance = math.length(targetTranslation.Value - translation.Value);
                    
                    if(distance + k_DistanceEpsilon > followTranslation.Distance)
                    {
                        if (speed.Value < speedOfTarget.Value + k_MaxRelativeSpeedDifference)
                        {
                            speed.Value += k_SpeedIncrease;
                        }
                    }
                    else if(distance - k_DistanceEpsilon < followTranslation.Distance)
                    {
                        if (speed.Value > speedOfTarget.Value - k_MaxRelativeSpeedDifference)
                        {
                            speed.Value -= k_SpeedDecrease;
                            if (speed.Value < 0)
                            {
                                speed.Value = 0;
                            }
                        }
                    }

                    speedArray[entityInQueryIndex] = speed;
                })
                .Schedule(inputDeps);

            jobHandle = Entities
                .WithDeallocateOnJobCompletion(speedArray)
                .WithAll<FollowTranslation, Translation>()
                .ForEach((int entityInQueryIndex, ref Speed speed) =>
                {
                    speed = speedArray[entityInQueryIndex];
                    
                }).Schedule(jobHandle);

            return jobHandle;
        }
    }
}