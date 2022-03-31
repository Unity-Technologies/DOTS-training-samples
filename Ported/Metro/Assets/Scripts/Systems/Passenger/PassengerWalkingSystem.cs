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

        Entities.ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, in PassengerWalking passengerWalking) =>
        {
            float remainingDistance = math.distance(passengerWalking.WalkDestination, translation.Value);
            if (remainingDistance < PASSENGER_WALK_EPSILON)
            {
                // When we reach our destination, remove walking component.
                parallelWriter.RemoveComponent<PassengerWalking>(entityInQueryIndex, entity);
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
