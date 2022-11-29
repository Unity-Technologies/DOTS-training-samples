using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    
    [BurstCompile]
    [WithAll(typeof(Train))]
    partial struct TrainMovementJob : IJobEntity
    {
        // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
        public float DeltaTime;
    
        void Execute(ref TrainAspect train)
        {
            train.TrainDirection = train.TrainDestination - train.Position.xz;
        }
    }

    
    [BurstCompile]
    public partial struct TrainMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var TrainMovementJob = new TrainMovementJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            TrainMovementJob.ScheduleParallel();
        }
    }
}