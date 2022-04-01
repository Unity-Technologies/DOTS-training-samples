using System.Globalization;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class PassengerWalkingSystem : SystemBase
{
    public const float PASSENGER_WALK_EPSILON = 0.001f;
    
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate()
    {
        m_EndSimulationEcbSystem = m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        var parallelWriter = ecb.AsParallelWriter();
        var worldTime = World.Time.ElapsedTime * 10;

        Entities.ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref PassengerWalking passengerWalking, in Passenger passenger) =>
        {
            var random = new Random((uint)(worldTime + ((entityInQueryIndex+1))));
            float remainingDistance = math.distance(passengerWalking.WalkDestination, translation.Value);
            if (remainingDistance < PASSENGER_WALK_EPSILON)
            {
                // When we reach our destination, re-randomize walking component.
                float3 destination = random.NextFloat3(
                    new float3(passenger.CurrentPlatformPosition.x - 10, 0, passenger.CurrentPlatformPosition.z - 10),
                    new float3(passenger.CurrentPlatformPosition.x + 10, 0, passenger.CurrentPlatformPosition.z + 10));

                parallelWriter.SetComponent(entityInQueryIndex, entity, new PassengerWalking
                {
                    WalkDestination = destination,
                    WalkSpeed = passengerWalking.WalkSpeed
                });
            }
            else
            {
                float3 direction = math.normalize(passengerWalking.WalkDestination - translation.Value);
                translation.Value += direction * math.min(remainingDistance, passengerWalking.WalkSpeed);
            }
            
        }).ScheduleParallel();
        
        // Ensure parallel writes to ECB are handled.
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
