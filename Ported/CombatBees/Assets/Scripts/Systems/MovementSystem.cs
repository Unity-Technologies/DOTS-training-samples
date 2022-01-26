using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial class MovementSystem : SystemBase
{
    private EntityCommandBufferSystem sys;

    protected override void OnCreate()
    {
        sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<Spawner>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = UnityEngine.Time.deltaTime;
        var tNow = UnityEngine.Time.timeSinceLevelLoad;
        var spawner = GetSingleton<Spawner>();

        var randomSeed = (uint) math.max(1,
            System.DateTime.Now.Millisecond +
            System.DateTime.Now.Second +
            System.DateTime.Now.Minute +
            System.DateTime.Now.Day +
            System.DateTime.Now.Month +
            System.DateTime.Now.Year);

        var random = new Random(randomSeed);
        
        // Move: Bee Bits
        Entities
            .WithAll<BeeBitsTag>()
            .ForEach((Entity e, ref Translation translation, ref Velocity velocity) =>
            {
                // accelerate towards ground with gravity
                if (translation.Value.y > 0)
                {
                    // accelerate velocity by gravity constant
                    velocity.Value.y -= 9.8f * deltaTime;

                    // move by velocity delta
                    translation.Value += velocity.Value * deltaTime;
                }
            }).Schedule();

        // Move: anything that moves (interpolates) between start and end points
        // - at present, the only thing that doesn't do this is Bee Bits, as it uses a velocity approach
        Entities
            .WithNone<BeeBitsTag>()
            .ForEach((Entity e, ref Translation translation, ref PP_Movement ppMovement) =>
            {
                translation.Value = ppMovement.Progress(deltaTime);
            }).Schedule();

        // Collision: Food
        var ecb = sys.CreateCommandBuffer();
        Entities
            .WithAny<FoodTag>()
            .ForEach((Entity e, ref Translation translation, ref PP_Movement ppMovement) =>
            {
                // Inside a Goal (hive) region
                if (math.abs(translation.Value.x) >= spawner.ArenaExtents.x + 0.5f)
                {
                    if (translation.Value.y <= 0.5f)
                    {
                        //destroy this entity and create/init a blood splat
                        ecb.DestroyEntity(e);


                        for (var i = 0; i < 3; i++)
                        {
                            var minBeeBounds = SpawnerSystem.GetBeeMinBounds(spawner);
                            var maxBeeBounds = SpawnerSystem.GetBeeMaxBounds(spawner, minBeeBounds);

                            var beeRandomY = SpawnerSystem.GetRandomBeeY(ref random, minBeeBounds, maxBeeBounds);
                            var beeRandomZ = SpawnerSystem.GetRandomBeeZ(ref random, minBeeBounds, maxBeeBounds);

                            if (translation.Value.x > 0)
                            {
                                // Yellow Bees
                                var beeRandomX =
                                    SpawnerSystem.GetRandomYellowBeeX(ref random, minBeeBounds, maxBeeBounds);

                                SpawnerSystem.BufferEntityInstantiation(spawner.YellowBeePrefab,
                                    new float3(beeRandomX, beeRandomY, beeRandomZ),
                                    ref ecb);
                            }
                            else
                            {
                                // Blue Bees
                                var beeRandomX =
                                    SpawnerSystem.GetRandomBlueBeeX(ref random, minBeeBounds, maxBeeBounds);

                                SpawnerSystem.BufferEntityInstantiation(spawner.BlueBeePrefab,
                                    new float3(beeRandomX, beeRandomY, beeRandomZ),
                                    ref ecb);
                            }
                        }
                    }
                }
            }).Schedule();
        
        // Collision: Bee Bits
        Entities
            .WithAll<BeeBitsTag>()
            .ForEach((Entity e, ref Translation translation, ref Velocity velocity) =>
            {
                // Ground Collision
                if (translation.Value.y < 0)
                {
                    // TODO: Spawn blood
                }
            }).Schedule();

        sys.AddJobHandleForProducer(Dependency);
    }
}