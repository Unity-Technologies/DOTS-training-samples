using MetroECS.Trains.States;
using Unity.Entities;

namespace MetroECS.Trains
{
    [UpdateAfter(typeof(TrainGenerationSystem))]
    public class TrainWaitingSystem : SystemBase
    {
        private EntityCommandBufferSystem sys;
        
        protected override void OnCreate()
        {
            sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = sys.CreateCommandBuffer();
            var time = Time.ElapsedTime;

            Entities.ForEach((ref Train train, in Entity trainEntity, in TrainWaitingTag trainWaitingData) =>
            {
                train.deltaPos = 0.0f;
                var timeWaiting = time - trainWaitingData.TimeStartedWaiting;
                if (timeWaiting >= trainWaitingData.TimeToWait)
                {
                    ecb.RemoveComponent<TrainWaitingTag>(trainEntity);
                    ecb.AddComponent(trainEntity, new TrainDoorsClosingTag());
                }
            }).Schedule();
            
            sys.AddJobHandleForProducer(Dependency);
        }
    }
}