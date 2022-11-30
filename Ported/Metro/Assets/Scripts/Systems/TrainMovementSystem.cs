using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [WithAll(typeof(Train))]
    partial struct SpeedController : IJobEntity
    {
        [ReadOnly] public ComponentLookup<MetroLine> m_MetroLine;
        [ReadOnly] public ComponentLookup<WorldTransform> WorldTransformFromEntity;

        void Execute(ref TrainSpeedControllerAspect train)
        {
            var metroLine = m_MetroLine[train.MetroLine];
            var nextTrain = metroLine.GetNextTrain(train.Index);
            var nextTrainPosition = WorldTransformFromEntity[nextTrain].Position;
            var distancesqr = math.distancesq(train.Position, nextTrainPosition);
            if (distancesqr < train.Speed * 10f)
                train.Speed = math.min(train.Speed - 0.95f * train.MaxSpeed, 0.1f);
            else
            {
                var distanceToDestination = math.distance(train.Destination, train.Position);
                if (distanceToDestination > train.Speed * 10f)
                    train.Speed = math.max(train.Speed + 0.95f * train.MaxSpeed, train.MaxSpeed);
                else if(train.DestinationType == RailwayPointType.Platform)
                    train.Speed = math.min(train.Speed - 0.95f * train.MaxSpeed, 0.1f);
            }
        }
    }

    [BurstCompile]
    [WithAll(typeof(Train))]
    partial struct TrainMovementJob : IJobEntity
    {
        // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
        public float DeltaTime;

        void Execute(ref TrainAspect train)
        {
            var direction = train.TrainDestination - train.Position;
            //train.Train.ValueRW.DestinationDirection = direction;
            var trainDirection = train.Forward;
            //train.Train.ValueRW.Forward = trainDirection;
            var angle = Utility.Angle(trainDirection, direction);
            //train.Train.ValueRW.Angle = angle;
            if (angle > 0.01f)
                train.Rotation = quaternion.RotateY(angle);


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
        ComponentLookup<MetroLine> m_MetroLine;
        ComponentLookup<WorldTransform> WorldTransformFromEntity;

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