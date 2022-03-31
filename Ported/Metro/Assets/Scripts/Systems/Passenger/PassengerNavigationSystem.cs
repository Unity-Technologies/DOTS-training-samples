using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class PassengerNavigationSystem : SystemBase
{
    public const float PASSENGER_DESTINATION_EPSILON = 0.01f;
    
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
        
        // Passengers who are currently not walking, queueing, or riding need something to do.
        Entities
            .WithNone<PassengerWalking>()
            .WithNone<PassengerRiding>()
            .WithNone<PassengerQueueing>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation, in Passenger passenger) =>
            {
                var random = new Random((uint)(worldTime + ((entityInQueryIndex+1))));
                // spin the RNG a few times 
                for (int i = 0; i < random.NextInt(0, 5); i++)
                {
                    random.NextInt();
                }
                
                if (math.distance(translation.Value, passenger.FinalDestination) < PASSENGER_DESTINATION_EPSILON)
                {
                    //destroy entity, passenger finished the journey
                    parallelWriter.DestroyEntity(entityInQueryIndex, entity);
                }
                else
                {
                    //not yet at destination.
                    //make them walk somewhere random (just a quick hack at the moment)
                    parallelWriter.AddComponent(entityInQueryIndex, entity, new PassengerWalking
                    {
                        WalkDestination = translation.Value + random.NextFloat3(new float3(-10, 0, -10), new float3(10, 0, 10)),
                        WalkSpeed = passenger.WalkSpeed
                    });
                }
            }).ScheduleParallel();
        
        
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
