using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using Random = Unity.Mathematics.Random;

public enum CommuterState : int
{
    WALK,
    QUEUE,
    GET_ON_TRAIN,
    GET_OFF_TRAIN,
    WAIT_FOR_STOP,
}

public struct CommuterSpawn : IComponentData
{
    public Entity Prefab;
    public int Count;
    public uint Seed;
}

public struct RoutingData : IComponentData
{
    public float3 Destination;
    public CommuterState State;
}

public struct ColorData : IComponentData
{
    public float3 Value;
}

public class CommuterSpawnSystem : JobComponentSystem
{
    public const float ACCELERATION_STRENGTH = 0.01f;

    private EntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var ecb = ecbSystem.CreateCommandBuffer().ToConcurrent();
        var commuterSpawnJob = Entities.ForEach((Entity entity, CommuterSpawn spawn, int entityInQueryIndex) =>
        {
            var rand = new Unity.Mathematics.Random(spawn.Seed);
            var count = spawn.Count;
            for (var i = 0; i < count; i++)
            {
                var instance = ecb.Instantiate(entityInQueryIndex, spawn.Prefab);
                var x = math.floor(i / math.sqrt(count));
                var y = i % math.sqrt(count);
                var position = float3(x, 0, y);
                ecb.SetComponent(entityInQueryIndex, instance, new Translation {Value = position});
                
                // random size
                ecb.AddComponent(entityInQueryIndex, instance, new NonUniformScale
                {
                    Value = float3(1.0f, rand.NextFloat(0.25f, 1.5f), 1.0f)
                });
                
                ecb.AddComponent(entityInQueryIndex, instance, new MovementDerivatives
                {
                    Acceleration = ACCELERATION_STRENGTH * rand.NextFloat(0.8f, 2f),
                });

                // random speed
                ecb.AddComponent(entityInQueryIndex, instance, new RoutingData
                {
                    State = CommuterState.WALK,
                    Destination = float3(
                        rand.NextFloat(-100f, 100f),
                        rand.NextFloat(-10f, 10f),
                        rand.NextFloat(-100f, 100f))
                });
                
                // random Color
                ecb.AddComponent(entityInQueryIndex, instance, new ColorData
                {
                    Value = float3(rand.NextFloat(0f, 1f), rand.NextFloat(0f, 1f), rand.NextFloat(0f, 1f))
                });
            }
            
            ecb.DestroyEntity(entityInQueryIndex, entity);
        }).Schedule(inputDependencies);
        
        ecbSystem.AddJobHandleForProducer(commuterSpawnJob);
        
        return commuterSpawnJob;
    }
}

[UpdateAfter(typeof(TrainPositioningSystem))]
public class CommuterSystem : JobComponentSystem
{
    public const float FRICTION = 0.8f;
    public const float ARRIVAL_THRESHOLD = 0.02f;
    public const float QUEUE_PERSONAL_SPACE = 0.4f;
    public const float QUEUE_MOVEMENT_DELAY = 0.25f;
    public const float QUEUE_DECISION_RATE = 3f;

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var dt = Time.DeltaTime;
        var commuterUpdateJob = Entities
            .ForEach((ref RoutingData commuter, ref MovementDerivatives movement,
                      ref Translation translation, ref Rotation heading) =>
        {
            if (commuter.State == CommuterState.WALK)
            {
                if (Approach.Apply(ref translation.Value, ref movement.Speed, commuter.Destination,
                    movement.Acceleration, ARRIVAL_THRESHOLD, FRICTION))
                {
                    commuter.State = CommuterState.WAIT_FOR_STOP;
                }
            }
        }).Schedule(inputDependencies);
        
        return commuterUpdateJob;
    }
}