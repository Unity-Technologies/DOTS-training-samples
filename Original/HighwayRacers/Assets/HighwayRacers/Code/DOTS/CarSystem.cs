//#define ENABLE_TEST

using Unity.Entities;
using Unity.Jobs;

[AlwaysUpdateSystem]
public partial class CarSystem : JobComponentSystem
{
#if ENABLE_TEST
    bool firstTime = true;
    Random rnd = new Random();
#endif

    EntityQuery CarEntityQuery;

    protected override void OnCreate()
    {
        CarEntityQuery = GetEntityQuery(ComponentType.ReadOnly<CarBasicState>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var deltaTime = UnityEngine.Time.deltaTime;
        if (deltaTime == 0) // possible when the game is paused
            return inputDeps;

#if ENABLE_TEST
        var highway = new HighwayProperties()
        {
            numCars = 1000,
            highwayLength = 250.0f
        };
#else
        if (!HasSingleton<HighwayProperties>())
            return inputDeps;
        var highway = GetSingleton<HighwayProperties>();
#endif

        var numCars = highway.numCars;

#if ENABLE_TEST
        if (firstTime)
        {
            rnd.InitState(9999);
            // Spawn a bunch of Lane and Position components for testing purpose.
            for (int i = 0; i < numCars; ++i)
            {
                var e = EntityManager.CreateEntity();
                EntityManager.AddComponentData(e, new CarBasicState()
                {
                    Lane = rnd.NextFloat(0, 3),
                    Position = rnd.NextFloat(0, 500),
                    Speed = rnd.NextFloat(0, 5)
                });
            }
            firstTime = false;
        }
#endif

        var queryStructure = new CarQueryStructure(numCars);
        inputDeps = queryStructure.Build(this, inputDeps, highway.highwayLength);

        inputDeps = new CarUpdateJob()
        {
            Dt = deltaTime,
            HighwayLen = highway.highwayLength,
            QueryStructure = queryStructure,
        }.Schedule(this, inputDeps);

        // 4. Compose the matrices for each mesh instance for correct rendering.

        // TODO: dispose the arrays on completing jobs.
        inputDeps.Complete();

        queryStructure.Dispose();

        return inputDeps;
    }
}
