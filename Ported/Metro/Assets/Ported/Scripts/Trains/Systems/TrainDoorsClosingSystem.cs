using MetroECS.Trains.States;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MetroECS.Trains
{
    public class TrainDoorsClosingSystem : SystemBase
    {
        private EntityCommandBufferSystem sys;
        
        protected override void OnCreate()
        {
            sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var ecb = sys.CreateCommandBuffer();
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

            sys.AddJobHandleForProducer(Dependency);
        }
    }
}