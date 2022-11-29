using System.Linq;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine.UI;

// Unmanaged systems based on ISystem can be Burst compiled, but this is not yet the default.
// So we have to explicitly opt into Burst compilation with the [BurstCompile] attribute.
// It has to be added on BOTH the struct AND the OnCreate/OnDestroy/OnUpdate functions to be
// effective.
[BurstCompile]
partial struct SpawnerSystem : ISystem
{
    // A ComponentLookup provides random access to a component (looking up an entity).
    // We'll use it to extract the world space position and orientation of the spawn point (cannon nozzle).
    ComponentLookup<Unity.Transforms.LocalToWorld> m_LocalToWorldTransformFromEntity;

    // Every function defined by ISystem has to be implemented even if empty.
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_LocalToWorldTransformFromEntity = state.GetComponentLookup<LocalToWorld>(true);
    }

    // Every function defined by ISystem has to be implemented even if empty.
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    // See note above regarding the [BurstCompile] attribute.
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //Instead of caching (as an option)
        //SystemAPI.GetComponentLookup<LocalTransform>();
        m_LocalToWorldTransformFromEntity.Update(ref state);
        
        //Debug.Log("SpawnerSystem OnUpdate");
        
        float dt = SystemAPI.Time.DeltaTime;

        NativeList<SpawnUnit> array = new NativeList<SpawnUnit>(state.WorldUpdateAllocator);

        
        foreach (var unitSpawner in SystemAPI.Query<RefRW<UnitSpawnerComponent>>())
        {
            unitSpawner.ValueRW.counter += dt;
            
            if (unitSpawner.ValueRO.counter > unitSpawner.ValueRO.frequency)
            {
                //Registering jobs for the entities to spawn
                var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

                for (int i = 0; i < unitSpawner.ValueRO.max; i++)
                {
                    var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
                    
                    var spawnJob = new SpawnUnit
                    {
                        LocalToWorldTransformFromEntity = m_LocalToWorldTransformFromEntity,
                        ECB = ecb
                    };
                    array.Add(spawnJob);
                }

                unitSpawner.ValueRW.counter = 0.0f;
            }
        }

        for (int i = 0; i < array.Length; i++)
        {
            var job = array[i];
            job.Schedule();
        }
        
        
    }
}

// Requiring the Shooting tag component effectively prevents this job from running
// for the tanks which are in the safe zone.
//[WithAll(typeof(Shooting))]
[BurstCompile]
partial struct SpawnUnit : IJobEntity
{
    [ReadOnly] public ComponentLookup<Unity.Transforms.LocalToWorld> LocalToWorldTransformFromEntity;
    public EntityCommandBuffer ECB;

    void Execute(in UnitSpawnerComponent unit)
    {
        var instance = ECB.Instantiate(unit.spawnObject);
        var spawnLocalToWorld = LocalToWorldTransformFromEntity[unit.spawnPoint].Position;
        var spawnTransform = LocalTransform.FromPosition(spawnLocalToWorld); //Unity.Transforms.WorldTransform.FromMatrix(spawnLocalToWorld.Value);

        ECB.SetComponent(instance, spawnTransform);



        /*
        var cannonBallTransform = UniformScaleTransform.FromPosition(spawnLocalToWorld.Value.Position);

        // We are about to overwrite the transform of the new instance. If we didn't explicitly
        // copy the scale it would get reset to 1 and we'd have oversized cannon balls.
        cannonBallTransform.Scale = LocalToWorldTransformFromEntity[turret.CannonBallPrefab].Value.Scale;
        ECB.SetComponent(instance, new LocalToWorldTransform
        {
            Value = cannonBallTransform
        });
        */
        
        Debug.Log("Execute me");
    }
    
    // Note that the TurretAspects parameter is "in", which declares it as read only.
    // Making it "ref" (read-write) would not make a difference in this case, but you
    // will encounter situations where potential race conditions trigger the safety system.
    // So in general, using "in" everywhere possible is a good principle.
    /*
    void Execute(in TurretAspect turret)
    {
        var instance = ECB.Instantiate(turret.CannonBallPrefab);
        var spawnLocalToWorld = LocalToWorldTransformFromEntity[turret.CannonBallSpawn];
        var cannonBallTransform = UniformScaleTransform.FromPosition(spawnLocalToWorld.Value.Position);

        // We are about to overwrite the transform of the new instance. If we didn't explicitly
        // copy the scale it would get reset to 1 and we'd have oversized cannon balls.
        cannonBallTransform.Scale = LocalToWorldTransformFromEntity[turret.CannonBallPrefab].Value.Scale;
        ECB.SetComponent(instance, new LocalToWorldTransform
        {
            Value = cannonBallTransform
        });
        ECB.SetComponent(instance, new CannonBall
        {
            Speed = spawnLocalToWorld.Value.Forward() * 20.0f
        });
        
        // The line below propagates the color from the turret to the cannon ball.
        ECB.SetComponent(instance, new URPMaterialPropertyBaseColor { Value = turret.Color });
    }
    */
}