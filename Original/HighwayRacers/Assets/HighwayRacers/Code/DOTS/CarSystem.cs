#define ENABLE_TEST

using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[AlwaysUpdateSystem]
public class CarSystem : JobComponentSystem
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
        const int carCount = 1000;

#if ENABLE_TEST
        if (firstTime)
        {
            rnd.InitState(9999);
            // Spawn a bunch of Lane and Position components for testing purpose.
            for (int i = 0; i < carCount; ++i)
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

        var queryStructure = new CarQueryStructure(carCount);
        inputDeps = queryStructure.Build(this, inputDeps, 250.0f);

        // 3. Car logic {Entity, index, velocity, state }
        // 4. Compose the matrices for each mesh instance for correct rendering.

        // TODO: dispose the arrays on completing jobs.
        inputDeps.Complete();

        queryStructure.Dispose();

        return inputDeps;
    }
}
