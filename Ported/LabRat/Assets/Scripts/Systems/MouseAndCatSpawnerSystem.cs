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
        MousePrefabs mousePrefabs = GetSingleton<MousePrefabs>();
        Direction curDirection = new Direction { Value = DirectionEnum.East };
        Position curPosition = new Position() { Value = new int2(0, 0) };

        // Need to spawn a mouse or a cat with an initial position and direction
        EntityCommandBufferSystem sys = this.World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer ecb = sys.CreateCommandBuffer();
        Entity curEntity = ecb.Instantiate(mousePrefabs.mousePrefab);
        ecb.AddComponent<MouseTag>(curEntity);
        ecb.AddComponent<Position>(curEntity, curPosition);
        ecb.AddComponent<Direction>(curEntity, curDirection);
    }
}
