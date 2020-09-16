using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class CatTag : IComponentData
{
}

public class MouseTag : IComponentData
{
}

public class MouseAndCatSpawnerSystem : SystemBase
{
    //float counter = 0f;  // ?
    //int totalSpawned = 0; // How many in flight?
    float frequency = 0.2f;
    float ticks = 0.0f;
    const float eps = 0.01f;

    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<MousePrefabs>();
        RequireSingletonForUpdate<CatPrefabs>();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        ticks = frequency;
    }

    protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     float deltaTime = Time.DeltaTime;
        ticks += eps;
        if (ticks < frequency)
        {
            return;
        }
        ticks -= frequency;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.
        MousePrefabs mousePrefabs = GetSingleton<MousePrefabs>();
        CatPrefabs catPrefabs = GetSingleton<CatPrefabs>();

        // Need to spawn a mouse or a cat with an initial position and direction
        EntityCommandBufferSystem sys = this.World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer ecb = sys.CreateCommandBuffer();
        float lTicks = ticks;
        float lFrequency = frequency;
        Entities.ForEach((ref Position position, in MouseAndCatSpawnerTag spawnerTag) => {
            Direction aDirection = new Direction { Value = DirectionEnum.East };
            Direction bDirection = new Direction { Value = DirectionEnum.West };
            PositionOffset curOffset = new PositionOffset() { Value = 0.5f };
            Speed curSpeed = new Speed() { Value = 2.0f };

            // Need to spawn a mouse or a cat with an initial position and direction
            Entity curEntity = ecb.Instantiate(mousePrefabs.mousePrefab);
            ecb.AddComponent<MouseTag>(curEntity);
            ecb.AddComponent<Position>(curEntity, position);
            ecb.AddComponent<PositionOffset>(curEntity, curOffset);
            ecb.AddComponent<Direction>(curEntity, (position.Value.x != 0) ? bDirection : aDirection);
            ecb.AddComponent<Speed>(curEntity, curSpeed);
        }).Schedule();
    }
}
