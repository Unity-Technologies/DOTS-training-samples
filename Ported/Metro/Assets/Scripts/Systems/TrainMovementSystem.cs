using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

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
            var direction = train.TrainDestination - train.Position;
            var distanceToThePoint = math.lengthsq(direction);
            if (distanceToThePoint > 0.001f)
            {
                train.Position += math.normalize(direction) * (DeltaTime * train.CurrentSpeed);
            }
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
            var trainMovementJob = new TrainMovementJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            trainMovementJob.ScheduleParallel();
        }
    }
}