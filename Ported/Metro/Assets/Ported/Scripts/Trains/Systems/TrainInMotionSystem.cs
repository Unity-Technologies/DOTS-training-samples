using MetroECS.Trains.States;
using Unity.Collections;
using Unity.Entities;
using Random = Unity.Mathematics.Random;

namespace MetroECS.Trains
{
    [UpdateAfter(typeof(TrainGenerationSystem))]
    public class TrainInMotionSystem : SystemBase
    {
        private EntityCommandBufferSystem sys;
        
        protected override void OnCreate()
        {
            sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var ecb = sys.CreateCommandBuffer();

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
                            var random = new Random(1234);
                            ecb.RemoveComponent<TrainInMotionTag>(trainEntity);
                            ecb.AddComponent(trainEntity, new TrainDoorsOpeningTag());
                        }
                    }
                }
            }).Schedule();

            sys.AddJobHandleForProducer(Dependency);
        }
    }
}