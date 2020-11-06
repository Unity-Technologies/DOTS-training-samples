using MetroECS.Trains.States;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MetroECS.Trains
{
    public class TrainDoorsClosingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var deltaTime = Time.DeltaTime;

            Entities.ForEach((ref TrainDoorsClosingTag closingData, in Entity trainEntity, in Train trainData) =>
            {
                if (closingData.Progress == 1f)
                {
                    ecb.RemoveComponent<TrainDoorsClosingTag>(trainEntity);
                    ecb.AddComponent(trainEntity, new TrainInMotionTag());
                }

                closingData.Progress = math.clamp(closingData.Progress + deltaTime, 0f, 1f);
            }).Run();

            ecb.Playback(EntityManager);
        }
    }
}