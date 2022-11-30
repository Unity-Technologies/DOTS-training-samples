using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct TrainSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Train>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var trainConfig = SystemAPI.GetSingleton<TrainConfig>();

            foreach (var (trainSpawn, entity) in SystemAPI.Query<Train>().WithEntityAccess())
            {
                var carriages = CollectionHelper.CreateNativeArray<Entity>(trainConfig.CarriageCount, Allocator.Temp);
                ecb.Instantiate(trainConfig.CarriagePrefab, carriages);

                for (int i = 0; i < carriages.Length; i++)
                {
                    var carriageEntity = carriages[i];
                    var carriage = new Carriage
                    {
                        Index = i,
                        Train = entity
                    };
                    ecb.AddComponent(carriageEntity, carriage);
                }
            }

            state.Enabled = false;
        }
    }
}