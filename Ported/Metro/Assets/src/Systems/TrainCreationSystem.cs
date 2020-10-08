using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(LineCreationSystem))]
public class TrainCreationSystem : SystemBase
{
    private EntityQuery linesToFill;
    private EntityArchetype trainArchetype;
    
    protected override void OnCreate()
    {
        linesToFill = GetEntityQuery(ComponentType.ReadOnly<TrainCount>(), ComponentType.ReadOnly<RailLength>());
        trainArchetype = EntityManager.CreateArchetype(typeof(Position), typeof(NextTrain), typeof(Rail), typeof(MaxPosition));
    }

    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in TrainCount trainCount, in RailLength railLength) =>
        {
            NativeArray<Entity> trains = new NativeArray<Entity>(trainCount, Allocator.Temp);
            
            for (int i = 0; i < trainCount; i++)
            {
                var startingPosition = (float)i / trainCount.Value * railLength.Value;
                var train = EntityManager.CreateEntity(trainArchetype);
                EntityManager.SetComponentData<Position>(train, startingPosition);
                EntityManager.SetComponentData<Rail>(train, entity);

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
        }).Run();

        EntityManager.RemoveComponent<TrainCount>(linesToFill);
    }
}
