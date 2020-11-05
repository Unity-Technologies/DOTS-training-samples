using MetroECS.Trains.States;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace MetroECS.Trains
{
    [UpdateAfter(typeof(TrainGenerationSystem))]
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
                    
                    Debug.Log("Leaving platform");
                }
            }).Run();
            
            ecb.Playback(EntityManager);
        }
    }
}