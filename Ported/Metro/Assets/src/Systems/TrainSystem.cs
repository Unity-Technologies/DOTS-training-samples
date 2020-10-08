using Unity.Entities;
using Unity.Mathematics;

public class TrainSystem : SystemBase
{
    private EntityCommandBufferSystem m_EcbSystem;
    
    protected override void OnCreate()
    {
        m_EcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        var ecb = m_EcbSystem.CreateCommandBuffer().AsParallelWriter();
        
        Entities
            .ForEach((ref NextPlatformPosition nextPlatformPosition, in Position position, in Rail rail, in NextPlatform nextPlatform) =>
            {
                var railLength = GetComponent<RailLength>(rail);
                var newNextPlatformPosition = (float)GetComponent<Position>(nextPlatform);
                if (newNextPlatformPosition < position)
                    newNextPlatformPosition += railLength;

                nextPlatformPosition = newNextPlatformPosition;
            }).ScheduleParallel();
        
        Entities
            .ForEach((ref NextTrainPosition nextTrainPosition, in Position position, in Rail rail, in NextTrain nextTrain) =>
            {
                var railLength = GetComponent<RailLength>(rail);
                var newNextTrainPosition = (float)GetComponent<Position>(nextTrain);
                if (newNextTrainPosition < position)
                    newNextTrainPosition += railLength;

                nextTrainPosition = newNextTrainPosition - 10f;
            }).ScheduleParallel();
        
        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref Position position, in Rail rail, in NextPlatformPosition nextPlatformPosition,
                      in NextPlatform nextPlatform, in NextTrainPosition nextTrainPosition) =>
            {
                var railLength = GetComponent<RailLength>(rail);
                var newPosition = position + 20f * deltaTime;
                newPosition = math.min(newPosition, nextTrainPosition);
                newPosition = math.min(newPosition, nextPlatformPosition);
                if (newPosition > railLength)
                    newPosition = math.frac(newPosition / railLength) * railLength;
                position = newPosition;

                if (newPosition == nextPlatformPosition)
                {
                    ecb.AddComponent(entityInQueryIndex, nextPlatform, new PlatformBoardingTrain {Train = entity});
                    ecb.RemoveComponent<NextPlatform>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
        
        m_EcbSystem.AddJobHandleForProducer(Dependency);
        
        Entities
            .ForEach((ref DynamicBuffer<BufferCarriage> carriages, in Position position, in Rail rail) =>
            {
                for (int i = 0; i < carriages.Length; i++)
                {
                    var carriagePosition = position - i * (TrainCarriage.CARRIAGE_LENGTH + TrainCarriage.CARRIAGE_SPACING);
                    if (carriagePosition < 0f)
                        carriagePosition += GetComponent<RailLength>(rail);
                    SetComponent<CarriagePosition>(carriages[i], carriagePosition);
                }
            }).Schedule();
    }
}
