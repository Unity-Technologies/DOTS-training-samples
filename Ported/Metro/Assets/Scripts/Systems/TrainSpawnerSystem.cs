using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(StationSpawnerSystem))]
public partial class TrainSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        Entities
            //.WithoutBurst() // Uncomment to debug the body of the Foreach
            .ForEach((Entity entity, in TrainSpawner spawner, in Spline spline, in EntityColor entityColor) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                var urpBaseColor = new URPMaterialPropertyBaseColor();
                urpBaseColor.Value = new float4(entityColor.Value.r,entityColor.Value.g,entityColor.Value.b,entityColor.Value.a);
                
                for (int i = 0; i < spawner.NoOfTrains; ++i)
                {
                    var leaderInstance = ecb.Instantiate(spawner.LeaderPrefab);
                    var trackProgress = new TrackProgress{Value = spline.splinePath.Value.pathLength * i / spawner.NoOfTrains};
                    var splineFollower = new SplineFollower{track = entity};
                    

                    var stationDistanceBuffer = GetBuffer<FloatBufferElement>(entity);
                    int stationIndex = 0;
                    int previousStation = stationDistanceBuffer.Length - 1;
                    //UnityEngine.Debug.Log($"starting search at: {trackProgress.Value}");
                    for (int j = stationDistanceBuffer.Length - 1; j >= 0; --j)
                    {
                        //UnityEngine.Debug.Log($"{stationDistanceBuffer[j].Value}");
                        if (trackProgress.Value > stationDistanceBuffer[j].Value)
                        {
                            previousStation = j;
                            stationIndex = (j + 1) % stationDistanceBuffer.Length;
                            break;
                        }
                    }
                    //UnityEngine.Debug.Log($"Found station {stationIndex} at {stationDistanceBuffer[stationIndex].Value}");
                    ref var splineData = ref spline.splinePath.Value;
                    
                    float startLocation = stationDistanceBuffer[previousStation];
                    float endLocation = stationDistanceBuffer[stationIndex];
                    if (startLocation>endLocation) startLocation -= splineData.pathLength;
                    if (trackProgress.Value > endLocation) trackProgress.Value -= splineData.pathLength;
                    
                    ecb.SetComponent(leaderInstance, trackProgress);
                    ecb.SetComponent(leaderInstance, splineFollower);
                    ecb.AddComponent<URPMaterialPropertyBaseColor>(leaderInstance);
                    ecb.SetComponent(leaderInstance, urpBaseColor);
                    ecb.AddComponent<Spline>(leaderInstance, spline);
                    ecb.SetComponent(leaderInstance, new CurrentRoute { 
                        targetStationIndex = stationIndex, 
                        routeStartLocation =  startLocation, 
                        routeEndLocation =  endLocation,
                        state = TrainState.InTransit
                    });

                    for (int j = 0; j < spawner.NoOfCartPerTrain; ++j)
                    {
                        var followerInstance = ecb.Instantiate(spawner.FollowerPrefab);
                        var follower = new Follower{Leader = leaderInstance, TrackData = entity, CartIndexInTrain = j+1};
                        ecb.SetComponent(followerInstance, follower);
                        ecb.AddComponent<URPMaterialPropertyBaseColor>(followerInstance);
                        ecb.SetComponent(followerInstance, urpBaseColor);
                        ecb.AddComponent<Spline>(followerInstance, spline);
                    }
                }
                ecb.RemoveComponent<TrainSpawner>(entity);
            }).Run();
        
        Entities
            .ForEach((Entity entity, in URPMaterialPropertyBaseColor baseColor) =>
            {
                
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}