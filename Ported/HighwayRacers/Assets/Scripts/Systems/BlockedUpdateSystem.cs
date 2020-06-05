using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BlockedUpdateSystem : SystemBase
{
    EntityCommandBufferSystem entityCommandBufferSystem;

    protected override void OnCreate()
    {
        entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>(); 
    }

    protected override void OnUpdate()
    {
        TrackProperties trackProperties = GetSingleton<TrackProperties>();
        CarConfigurations carConfig = GetSingleton<CarConfigurations>();
        float dtime = Time.DeltaTime;

        var ecb = entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        var minDistanceToFront = carConfig.MinDistanceToFront;

        Entities
            .WithNone<BlockedState>()
            .ForEach((int entityInQueryIndex, Entity carEntity, 
                ref TrackPosition trackPosition, 
                in CarInFront carInFront, in Speed speed, in CarProperties carProperties) =>
        {
            bool blocked = CheckBlock(speed.Value, carInFront.Speed, carConfig.Deceleration, 
                trackPosition.TrackProgress, carInFront.TrackProgressCarInFront,trackProperties.TrackLength,
                minDistanceToFront);

           if (blocked) {
                ecb.AddComponent<BlockedState>(entityInQueryIndex, carEntity);
           }
        }).ScheduleParallel();
    
        Entities
            .WithAll<BlockedState>()
            .ForEach((int entityInQueryIndex, Entity carEntity, 
                ref TrackPosition trackPosition, 
                in CarInFront carInFront, in Speed speed, in CarProperties carProperties) =>
        {
            bool blocked = CheckBlock(speed.Value, carInFront.Speed, carConfig.Deceleration, 
                trackPosition.TrackProgress, carInFront.TrackProgressCarInFront,trackProperties.TrackLength,
                minDistanceToFront);

            if (!blocked) {
                ecb.RemoveComponent<BlockedState>(entityInQueryIndex, carEntity);        
            } 
        }).ScheduleParallel();

        entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    static bool CheckBlock(float velocity, float velocityOfCarInFront, float decelerationSpeed, float trackProgress, float trackProgressCarInFront, float trackLength, float minDistanceToFront)
    {
        if (trackProgressCarInFront >= float.MaxValue)
            return false;

        float distanceBetweenCars = trackProgressCarInFront - trackProgress;
        distanceBetweenCars = (distanceBetweenCars + trackLength) % trackLength;

        if (velocity < velocityOfCarInFront)
            return false;

        float relativeVelocity = velocity - velocityOfCarInFront;

        float timeToSlowDown = relativeVelocity / decelerationSpeed;

        float spaceToSlowDown = relativeVelocity * timeToSlowDown - 0.5f * decelerationSpeed * timeToSlowDown * timeToSlowDown;

        float diff = distanceBetweenCars - spaceToSlowDown;

        float threshold = minDistanceToFront + 0.25f;
        return diff <= threshold;
    }
}