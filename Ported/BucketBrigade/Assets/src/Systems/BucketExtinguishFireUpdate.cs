using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class BucketExtinguishFireUpdate : SystemBase
    {
        EndSimulationEntityCommandBufferSystem m_EndSimulationEntityCommandBufferSystem;
        protected override void OnCreate()
        {
            base.OnCreate();
            m_EndSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

            RequireSingletonForUpdate<Temperature>();
        }

        protected override void OnUpdate()
        {
            var configEntity = GetSingletonEntity<FireSimConfig>();
            var configValues = GetComponent<FireSimConfigValues>(configEntity);

            var ecb = m_EndSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var concurrentEcb = ecb.AsParallelWriter();

            var temperatureEntity = GetSingletonEntity<Temperature>();
            DynamicBuffer<Temperature> temperatureBuffer = EntityManager.GetBuffer<Temperature>(temperatureEntity);
            NativeArray<Temperature> temperatureArray = temperatureBuffer.AsNativeArray();

            int splashRadius = configValues.SplashRadius;

            Entities//.WithBurst()
                .WithName("BucketExtinguishingFire")
                .WithAll<ThrowBucketAtFire, FullBucketTag>()
                .ForEach((int entityInQueryIndex, Entity bucketEntity, ref ThrowBucketAtFire throwBucketAtFire, ref EcsBucket bucket) =>
                {
                    bucket.WaterLevel = 0;
                    concurrentEcb.RemoveComponent<FullBucketTag>(entityInQueryIndex, bucketEntity);
                    concurrentEcb.RemoveComponent<ThrowBucketAtFire>(entityInQueryIndex, bucketEntity);

                    int2 fireCellPosition = configValues.GetColRowOfPosition2D(throwBucketAtFire.firePosition);                    

                    for (int y = Math.Max(0, fireCellPosition.y - splashRadius);
                         y <= Math.Min(fireCellPosition.y + splashRadius, configValues.Rows - 1);
                         ++y)
                    {
                        for (int x = Math.Max(0, fireCellPosition.x - splashRadius);
                             x <= Math.Min(fireCellPosition.x + splashRadius, configValues.Columns - 1);
                             ++x)
                        {
                            float intensity = temperatureArray[x + y * configValues.Columns].Intensity;

                            if (intensity > 0)
                            {

                                float2 splashPosition = configValues.GetPosition2DOfColRow(x, y);

                                float distance = math.length(splashPosition - throwBucketAtFire.firePosition);

                                float falloff = configValues.CoolingStrength_Falloff * distance;

                                float coolingStrength = Math.Max(0, configValues.CoolingStrength - falloff);

                                intensity = Math.Max(0, intensity - coolingStrength);

                                temperatureArray[x + y * configValues.Columns] = new Temperature { Intensity = intensity };
                            }
                        }
                    }

                }).Run();

            m_EndSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
