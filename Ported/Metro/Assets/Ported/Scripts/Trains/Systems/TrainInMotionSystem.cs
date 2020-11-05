using MetroECS.Trains.States;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MetroECS.Trains
{
    [UpdateAfter(typeof(TrainGeneration))]
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
                trainData.Position = (trainData.Position + (deltaTime * Train.MAX_SPEED)) % 1;
                
                var pathData = GetComponent<PathDataRef>(trainData.Path).ToNativePathData();
                var regionIndex = BezierHelpers.GetRegionIndex(pathData.Positions, pathData.Distances,
                    trainData.Position * pathData.TotalDistance);
                var markerType = pathData.MarkerTypes[regionIndex];

                if (markerType == (int) RailMarkerType.PLATFORM_END)
                {
                    var previousMarkerType = pathData.MarkerTypes[regionIndex - 1];
                    if (previousMarkerType == (int) RailMarkerType.PLATFORM_START)
                    {
                        var markerPosition = pathData.Distances[regionIndex] / pathData.TotalDistance;
                        if ((markerPosition - trainData.Position) < 0.1f)
                        {
                            var random = new Random(1234);
                            ecb.RemoveComponent<TrainInMotionTag>(trainEntity);
                            ecb.AddComponent(trainEntity, new TrainWaitingTag { TimeStartedWaiting = time, RandomWaitTime = random.NextFloat(5)});
                        }
                    }
                }
            }).Run();

            ecb.Playback(EntityManager);
        }
    }

    [UpdateAfter(typeof(TrainGeneration))]
    public class TrainWaitingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var time = Time.ElapsedTime;
            
            Entities.ForEach((in Entity trainEntity, in TrainWaitingTag trainWaitingData) =>
            {
                var timeWaiting = time - trainWaitingData.TimeStartedWaiting;
                if (timeWaiting >= trainWaitingData.RandomWaitTime)
                {
                    ecb.RemoveComponent<TrainWaitingTag>(trainEntity);
                    ecb.AddComponent(trainEntity, new TrainInMotionTag());
                }
            }).Run();
            
            ecb.Playback(EntityManager);
        }
    }
}

namespace MetroECS.Trains.States
{
    public struct TrainInMotionTag : IComponentData
    {
    }

    public struct TrainWaitingTag : IComponentData
    {
        public double TimeStartedWaiting;
        public float RandomWaitTime;
    }
}