using MetroECS.Trains.States;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace MetroECS.Trains
{
    [UpdateAfter(typeof(TrainGenerationSystem))]
    public class TrainInMotionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var time = Time.ElapsedTime;

            Entities
                .WithAll<TrainInMotionTag>()
                .ForEach((ref Train trainData, in Entity trainEntity) =>
            {
                trainData.deltaPos = deltaTime * Train.MAX_SPEED;
                trainData.Position = (trainData.Position + trainData.deltaPos);
                if (trainData.Position > 1.0f)
                    trainData.Position -= 1.0f;
                
                var pathData = GetComponent<PathDataRef>(trainData.Path).ToNativePathData();
                var regionIndex = BezierHelpers.GetRegionIndex(pathData.Positions, pathData.Distances,
                    trainData.Position * pathData.TotalDistance);

                if (regionIndex == trainData.TargetIndex)
                {
                    trainData.TargetIndex = (trainData.TargetIndex + 1) % pathData.MarkerTypes.Length;
                    
                    var markerType = pathData.MarkerTypes[regionIndex];
                    if (markerType == (int) RailMarkerType.PLATFORM_END)
                    {
                        var previousMarkerIndex = MathHelpers.Loop(regionIndex - 1, pathData.MarkerTypes.Length);
                        var previousMarkerType = pathData.MarkerTypes[previousMarkerIndex];
                        if (previousMarkerType == (int) RailMarkerType.PLATFORM_START)
                        {
                            //Debug.Log("Stopping at platform");
                            
                            var random = new Random(1234);
                            ecb.RemoveComponent<TrainInMotionTag>(trainEntity);
                            ecb.AddComponent(trainEntity, new TrainWaitingTag { TimeStartedWaiting = time, RandomWaitTime = random.NextFloat(5, 10)});       
                        }
                    }
                }
            }).Run();

            ecb.Playback(EntityManager);
        }
    }
}