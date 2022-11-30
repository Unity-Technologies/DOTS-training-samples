using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    public partial struct MetroLineSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MetroLine>();
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
            foreach (var metroLine in SystemAPI.Query<MetroLine>())
            {
                for (int i = 0; i < metroLine.RailwayPositions.Length; i++)
                {
                    if(metroLine.RailwayTypes[i] != RailwayPointType.Route)
                        continue;
                    var train = ecb.Instantiate(trainConfig.TrainPrefab);
                    ecb.SetComponent(train, LocalTransform.FromPositionRotation(metroLine.RailwayPositions[i], metroLine.RailwayRotations[i]));
                    var nextIndex = i - 1 < 0 ? metroLine.RailwayPositions.Length - 1 : i - 1;
                    ecb.SetComponent(train, new Train
                    {
                        Destination = metroLine.RailwayPositions[nextIndex],
                        State = TrainState.EnRoute
                    });
                }
            }

            state.Enabled = false;
        }
    }
}