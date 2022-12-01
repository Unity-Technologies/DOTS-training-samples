using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(TrainMovementSystem))]
    public partial struct PassengerLoadingSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Train>();
            state.RequireForUpdate<TrainPositions>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (trainState, train, trainEntity) in SystemAPI.Query<TrainStateComponent, Train>().WithEntityAccess())
            {
                switch (trainState.State)
                {
                    case TrainState.Arrived:
                    {
                        var metroLine = SystemAPI.GetComponent<MetroLine>(train.MetroLine);
                        var platformID = metroLine.Platforms[train.DestinationIndex];
                        foreach (var (platformId, platformEntity) in SystemAPI.Query<PlatformId>().WithAll<Platform>().WithEntityAccess())
                        {
                            if (platformId.Value != platformID) continue;
                            SystemAPI.SetComponent(platformEntity, new TrainOnPlatform
                            {
                                Train = trainEntity
                            });
                            SystemAPI.SetComponent(trainEntity, new TrainStateComponent { State = TrainState.WaitingOnPlatform });
                            break;
                        }

                        break;
                    }
                    case TrainState.Departing:
                    {
                        break;
                    }
                    case TrainState.WaitingOnPlatform:
                    {
                        SystemAPI.SetComponent(trainEntity, new TrainStateComponent
                        {
                            State = TrainState.Departing
                        });
                        var metroLine = SystemAPI.GetComponent<MetroLine>(train.MetroLine);
                        var platformID = metroLine.Platforms[train.DestinationIndex];
                        foreach (var (platformId, platformEntity) in SystemAPI.Query<PlatformId>().WithAll<Platform>().WithEntityAccess())
                        {
                            if (platformId.Value != platformID) 
                                continue;
                            SystemAPI.SetComponent(platformEntity, new TrainOnPlatform { Train = Entity.Null });
                        }
                        break;
                    }
                }
            }
        }
    }
}