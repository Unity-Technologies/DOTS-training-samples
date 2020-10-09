using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
            .WithNone<NextPlatform>()
            .ForEach((Entity train, int entityInQueryIndex, in Rail rail, in Position position) =>
            {
                var platforms = GetBuffer<BufferPlatform>(rail);

                Entity chosenPlatform = platforms[0];
                for (int i = 0; i < platforms.Length; i++)
                {
                    Entity platform = platforms[i].Value;

                    float platformPosition = GetComponent<Position>(platform);
                    if (platformPosition > position)
                    {
                        chosenPlatform = platform;
                        break;
                    }
                }

                ecb.AddComponent(entityInQueryIndex, train, new NextPlatform() { Value = chosenPlatform });
            }).Schedule();

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
            .ForEach((ref NextTrainPosition nextTrainPosition, in Position position, in DynamicBuffer<BufferCarriage> carriages, in Rail rail, in NextTrain nextTrain) =>
            {
                var trainLength = (carriages.Length + 1) * (TrainCarriage.CARRIAGE_LENGTH + TrainCarriage.CARRIAGE_SPACING);
                
                var railLength = GetComponent<RailLength>(rail);
                var newNextTrainPosition = (float)GetComponent<Position>(nextTrain);
                if (newNextTrainPosition < position)
                    newNextTrainPosition += railLength;

                nextTrainPosition = newNextTrainPosition - trainLength;
            }).ScheduleParallel();
        
        Entities
            .WithAll<NextPlatform>()
            .WithNone<TrainTask_OpenDoors>()
            .WithNone<TrainTask_UnboardPassengers>()
            .WithNone<TrainTask_BoardPassengers>()
            .WithNone<TrainTask_CloseDoors>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Position position, in Rail rail, in NextPlatformPosition nextPlatformPosition, in NextTrainPosition nextTrainPosition) =>
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
                    ecb.AddComponent(entityInQueryIndex, entity, new TrainTask_OpenDoors() { TimeRemaining = 2f });
                }
            }).ScheduleParallel();

        Entities
            .ForEach((Entity train, int entityInQueryIndex, ref TrainTask_OpenDoors timeTask, in NextPlatform nextPlatform, in DynamicBuffer<BufferCarriage> carriages) =>
            {
                timeTask.TimeRemaining = math.max(timeTask.TimeRemaining - deltaTime, 0);

                for(int i = 0; i < carriages.Length; i++)
                {
                    var doors = GetComponent<CarriageDoors>(carriages[i]);

                    SetComponent(doors.DoorL_Geo, new Translation { Value = math.lerp(GetComponent<Translation>(doors.DoorL_Open).Value, GetComponent<Translation>(doors.DoorL_Closed).Value, timeTask.TimeRemaining / 2f) });
                    SetComponent(doors.DoorR_Geo, new Translation { Value = math.lerp(GetComponent<Translation>(doors.DoorR_Open).Value, GetComponent<Translation>(doors.DoorR_Closed).Value, timeTask.TimeRemaining / 2f) });
                }

                if (timeTask.TimeRemaining <= 0)
                {
                    ecb.AddComponent(entityInQueryIndex, nextPlatform, new PlatformUnboardingTrain { Train = train });

                    ecb.AddComponent(entityInQueryIndex, train, new TrainTask_UnboardPassengers() { TimeRemaining = 4f });
                    ecb.RemoveComponent<TrainTask_OpenDoors>(entityInQueryIndex, train);
                }
            }).Schedule();

        Entities
            .ForEach((Entity train, int entityInQueryIndex, ref TrainTask_UnboardPassengers timeTask, in NextPlatform nextPlatform) =>
            {
                timeTask.TimeRemaining -= deltaTime;

                if (timeTask.TimeRemaining <= 0)
                {
                    ecb.AddComponent(entityInQueryIndex, nextPlatform, new PlatformBoardingTrain { Train = train });

                    ecb.AddComponent(entityInQueryIndex, train, new TrainTask_BoardPassengers() { TimeRemaining = 4f });
                    ecb.RemoveComponent<TrainTask_UnboardPassengers>(entityInQueryIndex, train);
                }
            }).ScheduleParallel();

        Entities
            .ForEach((Entity train, int entityInQueryIndex, ref TrainTask_BoardPassengers timeTask) =>
            {
                timeTask.TimeRemaining -= deltaTime;

                if (timeTask.TimeRemaining <= 0)
                {
                    ecb.AddComponent(entityInQueryIndex, train, new TrainTask_CloseDoors() { TimeRemaining = 2f });
                    ecb.RemoveComponent<TrainTask_BoardPassengers>(entityInQueryIndex, train);
                }
            }).ScheduleParallel();

        Entities
            .ForEach((Entity train, int entityInQueryIndex, ref TrainTask_CloseDoors timeTask, in DynamicBuffer<BufferCarriage> carriages) =>
            {
                timeTask.TimeRemaining = math.max(timeTask.TimeRemaining - deltaTime, 0);

                for (int i = 0; i < carriages.Length; i++)
                {
                    var doors = GetComponent<CarriageDoors>(carriages[i]);

                    SetComponent(doors.DoorL_Geo, new Translation { Value = math.lerp(GetComponent<Translation>(doors.DoorL_Closed).Value, GetComponent<Translation>(doors.DoorL_Open).Value, timeTask.TimeRemaining / 2f) });
                    SetComponent(doors.DoorR_Geo, new Translation { Value = math.lerp(GetComponent<Translation>(doors.DoorR_Closed).Value, GetComponent<Translation>(doors.DoorR_Open).Value, timeTask.TimeRemaining / 2f) });
                }

                if (timeTask.TimeRemaining <= 0)
                {
                    ecb.RemoveComponent<TrainTask_CloseDoors>(entityInQueryIndex, train);
                    ecb.RemoveComponent<NextPlatform>(entityInQueryIndex, train);
                }
            }).Schedule();

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
