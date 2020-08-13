using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;

// Systems can schedule work to run on worker threads.
// However, creating and removing Entities can only be done on the main thread to prevent race conditions.
// The system uses an EntityCommandBuffer to defer tasks that can't be done inside the Job.

// ReSharper disable once InconsistentNaming
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class LineSpawnerSystem_FromEntity : SystemBase
{
    // BeginInitializationEntityCommandBufferSystem is used to create a command buffer which will then be played back
    // when that barrier system executes.
    // Though the instantiation command is recorded in the SpawnJob, it's not actually processed (or "played back")
    // until the corresponding EntityCommandBufferSystem is updated. To ensure that the transform system has a chance
    // to run on the newly-spawned entities before they're rendered for the first time, the SpawnerSystem_FromEntity
    // will use the BeginSimulationEntityCommandBufferSystem to play back its commands. This introduces a one-frame lag
    // between recording the commands and instantiating the entities, but in practice this is usually not noticeable.
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // Schedule the Entities.ForEach lambda job that will add Instantiate commands to the EntityCommandBuffer.
        // Since this job only runs on the first frame, we want to ensure Burst compiles it before running to get the best performance (3rd parameter of WithBurst)
        // The actual job will be cached once it is compiled (it will only get Burst compiled once).
        Entities
            .WithName("LineSpawnerSystem_FromEntity")
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .WithStructuralChanges()
            //.ForEach((Entity entity, int entityInQueryIndex, in LineSpawner_FromEntity lineSpawnerFromEntity, in LocalToWorld location) =>
            .ForEach((Entity spawnerEntity, in LineSpawner_FromEntity lineSpawnerFromEntity, in Translation translation) =>
        {
            var random = new Random(1);
            for (var x = 0; x < lineSpawnerFromEntity.Count; x++)
                {
                    var instance = EntityManager.Instantiate(lineSpawnerFromEntity.LinePrefab);
                    
                    var line = new Line();
                    var position = new Translation{Value = new float3(random.NextFloat(0, 10f),0, random.NextFloat(0, 10f))};
                    
                    for (var a = 0; a < lineSpawnerFromEntity.CountOfFullPassBots; a++)
                    {
                        var bot = EntityManager.Instantiate(lineSpawnerFromEntity.BotPrefab);
                        var botPosition = new Translation{Value = new float3(random.NextFloat(0, 10f), 0, random.NextFloat(0, 10f))};
                        EntityManager.AddComponentData(bot, botPosition);
                    }

                    for (var a = 0; a < lineSpawnerFromEntity.CountOfEmptyPassBots; a++)
                    {
                        var bot = EntityManager.Instantiate(lineSpawnerFromEntity.BotPrefab);
                        var botPosition = new Translation{Value = new float3(random.NextFloat(0, 10f), 0, random.NextFloat(0, 10f))};
                        EntityManager.AddComponentData(bot, botPosition);
                    }

                    EntityManager.AddComponentData(instance, position);
                }

            EntityManager.DestroyEntity(spawnerEntity);
        }).Run();

        // SpawnJob runs in parallel with no sync point until the barrier system executes.
        // When the barrier system executes we want to complete the SpawnJob and then play back the commands (Creating the entities and placing them).
        // We need to tell the barrier system which job it needs to complete before it can play back the commands.
        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
