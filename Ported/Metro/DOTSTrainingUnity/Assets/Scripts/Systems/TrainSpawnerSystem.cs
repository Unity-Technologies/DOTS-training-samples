using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class TrainSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        NativeArray<float> distances = Line.allDistances;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        Entities.ForEach((Entity entity, in TrainSpawnerComponent spawnerData) =>
        {
            for (int trackIdx = 0; trackIdx < spawnerData.numberOfTracks; ++trackIdx)
            {
                for (int trainIdx = 0; trainIdx < spawnerData.numberOfTrainsPerTrack; ++trainIdx)
                {
                    float trackDistance = distances[trackIdx];
                    
                    // spawn train
                    Entity newTrainEngine = ecb.Instantiate(spawnerData.TrainEnginePrefab);
                    
                    // need to set total distance, target distance, max speed, track index, train state
                    ecb.SetComponent(newTrainEngine, new TrackIndex(){value = trackIdx});
                    ecb.SetComponent(newTrainEngine, new TrainState(){value = CurrTrainState.Moving});
                    ecb.SetComponent(newTrainEngine, new TrainTotalDistance(){value = trackDistance});
                    ecb.SetComponent(newTrainEngine, new TrainCurrDistance(){value = trainIdx * (trackDistance / spawnerData.numberOfTrainsPerTrack) });
                    ecb.SetComponent(newTrainEngine, new TrainTargetDistance() {value = (trainIdx + 1) * (trackDistance / spawnerData.numberOfTrainsPerTrack) } );
                    ecb.SetComponent(newTrainEngine, new TrainMaxSpeed(){value = 100.0f});
                    
                    // spawn train cars
                    for (int carIdx = 0; carIdx < spawnerData.numberOfTrainCarsPerTrain; ++carIdx)
                    {
                        Entity newTrainCar = ecb.Instantiate(spawnerData.TrainCarPrefab);
                        ecb.SetComponent(newTrainCar, new TrainCarIndex(){value = carIdx});
                        ecb.SetComponent(newTrainCar, new TrainEngineRef(){value = newTrainEngine});
                        // eventually do color here
                        //dstManager.SetComponentData(entity, new Color() {value = new float4(color.r, color.g, color.b, color.a)});
                    }
                }
            }
        }).Run();
        
        Enabled = false;
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
