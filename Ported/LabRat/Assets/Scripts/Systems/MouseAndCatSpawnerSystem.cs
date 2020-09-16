using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;

public class MouseAndCatSpawnerSystem : SystemBase
{
    NativeReference<Random> randomRef;
    protected override void OnCreate()
    {
        base.OnCreate();
        var random = new Random((uint)System.DateTime.UtcNow.Millisecond + 1);
        randomRef = new NativeReference<Random>(Allocator.Persistent);
        randomRef.Value = random;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        randomRef.Dispose();
    }
    protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     float deltaTime = Time.DeltaTime;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.

        EntityCommandBufferSystem sys = this.World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer ecb = sys.CreateCommandBuffer();
        var lRandomRef = randomRef;
        float lEps = 0.01f;
        Entities.ForEach((ref MouseAndCatSpawnerData spawnerData, in Position position, in Direction direction, in Speed speed) => {
            spawnerData.ticks += lEps;
            if (spawnerData.ticks < spawnerData.frequency)
            {
                return;
            }
            var lRandom = lRandomRef.Value;
            float rand = math.clamp(lRandom.NextFloat(), 0.3f, 1.0f);
            lRandomRef.Value = lRandom;
            spawnerData.ticks -= spawnerData.frequency * rand;

            PositionOffset offset = new PositionOffset() { Value = 0.5f };
            Entity curEntity = ecb.Instantiate(spawnerData.prefabEntity);
            ecb.AddComponent<Position>(curEntity, position);
            ecb.AddComponent<PositionOffset>(curEntity, offset);
            ecb.AddComponent<Direction>(curEntity, direction);
            ecb.AddComponent<Speed>(curEntity, speed);
        }).Schedule();
        
    }
}
