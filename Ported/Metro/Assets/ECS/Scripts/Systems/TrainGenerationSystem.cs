using Unity.Entities;

public partial class TrainGenerationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var config = SystemAPI.GetSingleton<Config>();
        var entityManager = World.EntityManager;
        var trainArchetype = entityManager.CreateArchetype(typeof(Train));

        Entities
            .WithStructuralChanges()
            .ForEach((Line line, Entity lineEntity) =>
        {
            for (var t = 0; t < line.MaxTrains; t++)
            {
                var trainEntity = entityManager.CreateEntity(trainArchetype);
                var train = new Train {Line = lineEntity };
                SetComponent(trainEntity, train);

                for (var c = 0; c < line.CarriagesPerTrain; c++)
                {
                    var carriageEntity = entityManager.Instantiate(config.CarriagePrefab);
                    var carriage = GetComponent<Carriage>(carriageEntity);
                    carriage.Index = c;
                    carriage.Train = trainEntity;
                    
                    SetComponent(carriageEntity, carriage);
                }
            }
        }).Run();

        Enabled = false;
    }
}