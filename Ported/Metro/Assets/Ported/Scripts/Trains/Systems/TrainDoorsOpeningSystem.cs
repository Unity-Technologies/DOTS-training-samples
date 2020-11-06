using MetroECS.Trains.States;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MetroECS.Trains
{
    public class TrainDoorsOpeningSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((ref TrainDoorsOpeningTag openingData, in Entity trainEntity, in Train trainData) =>
            {
                if (openingData.Progress == 1f)
                {
                    ecb.RemoveComponent<TrainDoorsOpeningTag>(trainEntity);
                    ecb.AddComponent(trainEntity, new TrainWaitingTag {TimeStartedWaiting = deltaTime, TimeToWait = 5f});
                }

                openingData.Progress = math.clamp(openingData.Progress + deltaTime, 0f, 1f);
            }).Run();

            ecb.Playback(EntityManager);
        }
    }
}