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

    static bool CheckBlock(float velocity, float velocityOfCarInFront, float acceleration, float trackProgress, float trackProgressCarInFront, float trackLength) {
        float timeToSlowDown = (velocityOfCarInFront-velocity) / acceleration;
        float spaceToSlowDown = velocity * timeToSlowDown + 0.5f * acceleration * timeToSlowDown * timeToSlowDown;
    
        float threshold = 1.25f;

        float distanceBetweenCars = trackProgressCarInFront - trackProgress;
        distanceBetweenCars -= threshold; // XXX temp. replace

        // get distance and take track length into account
        distanceBetweenCars = (distanceBetweenCars + trackLength) % trackLength;

        return !(velocityOfCarInFront > velocity || distanceBetweenCars > spaceToSlowDown);
    }
}