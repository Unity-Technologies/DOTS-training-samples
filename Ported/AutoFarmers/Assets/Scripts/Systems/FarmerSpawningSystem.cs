using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    public class FarmerSpawningSystem : SystemBase
    {
        private static bool IsFirst = true;
        private EntityCommandBufferSystem m_CommandBufferSystem;

        protected override void OnCreate()
        {
            m_CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = m_CommandBufferSystem.CreateCommandBuffer();

            var storeSettings = GetSingleton<StoreSpawner>();
            int farmerCost = storeSettings.FarmerCost;
            int droneCost = storeSettings.DroneCost;
            Entity farmerPrefab = storeSettings.FarmerPrefab;
            Entity dronePrefab = storeSettings.DronePrefab;

            var pecb = ecb.ToConcurrent();

            Entities
            .WithAll<Store_Tag>()
            .ForEach((int entityInQueryIndex, Entity store, ref DepositedPlantCount plantsInStore, ref RandomSeed randomSeed, in Translation storePosition) =>
            {
                Random random = new Random(randomSeed.Value);

                float3 position = storePosition.Value;

                if (plantsInStore.ForFarmers >= farmerCost)
                {
                    float dir = random.NextFloat(0.0f, math.PI * 2.0f);
                    float3 velocity = new float3(math.cos(dir), 0.0f, math.sin(dir));

                    var instance = pecb.Instantiate(entityInQueryIndex, farmerPrefab);
                    pecb.SetComponent(entityInQueryIndex, instance, new Translation { Value = position });
                    pecb.SetComponent(entityInQueryIndex, instance, new Velocity { Value = velocity });
                    plantsInStore.ForFarmers -= farmerCost;

                    if (IsFirst)
                    {
                        pecb.AddComponent<CameraFollow_Tag>(entityInQueryIndex, instance);
                        IsFirst = false;
                    }
                }

                if (plantsInStore.ForDrones >= droneCost)
                {
                    float dir = random.NextFloat(0.0f, math.PI * 2.0f);
                    float3 velocity = new float3(math.cos(dir), 0.0f, math.sin(dir));

                    var instance = pecb.Instantiate(entityInQueryIndex, dronePrefab);
                    position.y = 1.2f;// start above the store
                    pecb.SetComponent(entityInQueryIndex, instance, new Translation { Value = position });
                    pecb.SetComponent(entityInQueryIndex, instance, new Velocity { Value = velocity });
                    plantsInStore.ForDrones -= droneCost;
                }

                randomSeed.Value = random.state;
            }).ScheduleParallel();

            m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}