using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class TrainSpawnerSystem : SystemBase
{
    private EntityQuery RequirePropagation;
    
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
                    ecb.SetComponent(newTrainEngine, new TrainCurrDistance(){value = trainIdx * (trackDistance / spawnerData.numberOfTrainsPerTrack) });
                    ecb.SetComponent(newTrainEngine, new TrainTargetDistance() {value = (trainIdx + 1) * (trackDistance / spawnerData.numberOfTrainsPerTrack) } );
                    ecb.SetComponent(newTrainEngine, new TrainMaxSpeed(){value = 100.0f});

                    // spawn train cars
                    for (int carIdx = 0; carIdx < spawnerData.numberOfTrainCarsPerTrain; ++carIdx)
                    {
                        Entity newTrainCar = ecb.Instantiate(spawnerData.TrainCarPrefab);
                        ecb.SetComponent(newTrainCar, new TrainCarIndex(){value = carIdx});
                        ecb.SetComponent(newTrainCar, new TrainEngineRef(){value = newTrainEngine});

                        int color = trackIdx % 4;
                        float4 theColor = spawnerData.color0;
                        switch (color)
                        {
                            case 1:
                            {
                                theColor = spawnerData.color1;
                                break;
                            }
                            case 2:
                            {
                                theColor = spawnerData.color2;
                                break;
                            }
                            case 3:
                            {
                                theColor = spawnerData.color3;
                                break;
                            }
                            default:
                                break;
                        };
                        ecb.SetComponent(newTrainCar, new Color() {value = theColor});
                    }
                }
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        ecb = new EntityCommandBuffer(Allocator.Temp);
        
        ecb.RemoveComponentForEntityQuery<PropagateColor>(RequirePropagation);
        var cdfe = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();
        Entities
            .WithNativeDisableContainerSafetyRestriction(cdfe)
            .WithStoreEntityQueryInField(ref RequirePropagation)
            .WithAll<PropagateColor>()
            .ForEach((in DynamicBuffer<LinkedEntityGroup> group
                , in Color color) =>
            {
                for (int i = 1; i < group.Length; ++i)
                {
                    if (HasComponent<URPMaterialPropertyBaseColor>(group[i].Value))
                    {
                        cdfe[group[i].Value] = new URPMaterialPropertyBaseColor() {Value = color.value};
                    }
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
         
        ecb = new EntityCommandBuffer(Allocator.Temp);

        // Add right doors
        Entities.ForEach((Entity entity, in TrainEngineRef trainEngineRef, in DoorsRef doorsRef) =>
        {
            ecb.AppendToBuffer<DoorEntities>(trainEngineRef.value, doorsRef.doorEntRight);
        }).Run();

        // Add left doors
        Entities.ForEach((Entity entity, in TrainEngineRef trainEngineRef, in DoorsRef doorsRef) =>
        {
            ecb.AppendToBuffer<DoorEntities>(trainEngineRef.value, doorsRef.doorEntLeft);
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        Enabled = false;
    }
}
