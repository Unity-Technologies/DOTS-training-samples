using src.DOTS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class TrainMovementSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_EndSimulationSystem;
    protected override void OnCreate()
    {
        m_EndSimulationSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     float deltaTime = Time.DeltaTime;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.

        var deltaTime = Time.DeltaTime;
        var metro = this.GetSingleton<GameObjectRefs>().metro;
        var ecb = m_EndSimulationSystem.CreateCommandBuffer();

        Entities
            .WithoutBurst()
            .WithNone<TrainWaiting>()
            .ForEach((Entity entity, ref Translation translation, ref Carriage carriage) => {
            // Implement the work to perform for each entity here.
            // You should only access data that is local or that is a
            // field on this job. Note that the 'rotation' parameter is
            // marked as 'in', which means it cannot be modified,
            // but allows this job to run in parallel with other jobs
            // that want to read Rotation component data.
            // For example,
            //     translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;

            translation.Value = metro.metroLines[carriage.LaneIndex].Get_PositionOnRail(carriage.PositionAlongTrack);
            if (metro.metroLines[carriage.LaneIndex].Get_RegionIndex(carriage.PositionAlongTrack) == metro.metroLines[carriage.LaneIndex]
                .platforms[carriage.NextPlatformIndex].point_platform_START.index)
            {
                var waiting = new TrainWaiting() {RemainingTime = 5.0f};
                ecb.AddComponent<TrainWaiting>(entity, waiting);
            }
            else
            {
                carriage.PositionAlongTrack += 0.001f;
                carriage.PositionAlongTrack %= 1;
            }
            }).Run();
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, ref TrainWaiting trainWaiting, ref Carriage carriage) =>
        {
            trainWaiting.RemainingTime -= deltaTime;
            if (trainWaiting.RemainingTime <= 0)
            {
                ecb.RemoveComponent<TrainWaiting>(entity);
                carriage.NextPlatformIndex = metro.metroLines[carriage.LaneIndex]
                    .Get_NextPlatformIndex(carriage.PositionAlongTrack, carriage.NextPlatformIndex);
                carriage.NextPlatformPosition = metro.metroLines[carriage.LaneIndex].platforms[carriage.NextPlatformIndex].point_platform_START
                    .distanceAlongPath;
            }
        }).Run();
    }
}
