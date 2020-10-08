using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(LineCreationSystem))]
public class TrainCreationSystem : SystemBase
{
    private EntityArchetype trainArchetype;
    
    protected override void OnCreate()
    {
        trainArchetype = EntityManager.CreateArchetype(typeof(Position), typeof(Rail), typeof(NextTrain), typeof(NextTrainPosition),
                                                       typeof(CarriageCount), typeof(BufferCarriage),
                                                       typeof(NextPlatform), typeof(NextPlatformPosition));
    }

    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
            .ForEach((Entity entity,
                      in TrainCount trainCount,
                      in CarriageCount carriageCount,
                      in RailLength railLength) =>
        {
            NativeArray<Entity> trains = new NativeArray<Entity>(trainCount, Allocator.Temp);
            
            for (int i = 0; i < trainCount; i++)
            {
                var startingPosition = (float)i / trainCount.Value * railLength.Value;
                var train = EntityManager.CreateEntity(trainArchetype);
                EntityManager.SetComponentData<Position>(train, startingPosition);
                EntityManager.SetComponentData<Rail>(train, entity);
                EntityManager.SetComponentData(train, carriageCount);
                EntityManager.SetComponentData<NextPlatform>(train, (Entity)EntityManager.GetBuffer<BufferPlatform>(entity)[0]);

                trains[i] = train;
            }

            for (int i = 0; i < trainCount; i++)
            {
                var nextTrainIndex = i + 1;
                if (nextTrainIndex == trainCount)
                    nextTrainIndex = 0;
                EntityManager.SetComponentData<NextTrain>(trains[i], trains[nextTrainIndex]);
            }

            trains.Dispose();

            EntityManager.RemoveComponent<TrainCount>(entity);
            EntityManager.RemoveComponent<CarriageCount>(entity);
        }).Run();
    }
}
