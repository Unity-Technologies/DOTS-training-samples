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
        float dtime = Time.DeltaTime;

        var ecb = entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            // .WithoutBurst()
            .WithNone<BlockedState>()
            .ForEach((int entityInQueryIndex, Entity carEntity, 
                ref TrackPosition trackPosition, 
                in CarInFront carInFront, in Speed speed, in CarProperties carProperties) =>
        {
            bool blocked = CheckBlock(speed.Value, carInFront.Speed, carProperties.Acceleration, 
                trackPosition.TrackProgress, carInFront.TrackProgressCarInFront,trackProperties.TrackLength);

           if (blocked) {
                ecb.AddComponent<BlockedState>(entityInQueryIndex, carEntity);
           }
        }).ScheduleParallel();
        // }).Run();
    
        Entities
            .WithAll<BlockedState>()
            .ForEach((int entityInQueryIndex, Entity carEntity, 
                ref TrackPosition trackPosition, 
                in CarInFront carInFront, in Speed speed, in CarProperties carProperties) =>
        {
            bool blocked = CheckBlock(speed.Value, carInFront.Speed, carProperties.Acceleration, 
                trackPosition.TrackProgress, carInFront.TrackProgressCarInFront,trackProperties.TrackLength);

            if (!blocked) {
                ecb.RemoveComponent<BlockedState>(entityInQueryIndex, carEntity);        
            } 
        }).ScheduleParallel();

        entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    static bool CheckBlock(float velocity, float velocityOfCarInFront, float acceleration, float trackProgress, float trackProgressCarInFront, float trackLength)
    {
        float distanceBetweenCars = trackProgressCarInFront - trackProgress;
        distanceBetweenCars = (distanceBetweenCars + trackLength) % trackLength;

        const float minDistance = 1.25f;
        if (distanceBetweenCars < minDistance)
            return true;

        if (velocity < velocityOfCarInFront)
            return false;

        float relativeVelocity = velocity - velocityOfCarInFront;

        float timeToSlowDown = relativeVelocity / acceleration;

        float spaceToSlowDown = relativeVelocity * timeToSlowDown - 0.5f * acceleration * timeToSlowDown * timeToSlowDown;

        float diff = distanceBetweenCars - spaceToSlowDown;

        const float threshold = 1.5f;
        return diff <= threshold;
    }
}