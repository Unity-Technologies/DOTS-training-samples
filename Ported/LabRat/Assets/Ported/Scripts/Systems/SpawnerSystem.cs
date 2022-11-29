using System.Linq;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;

// Unmanaged systems based on ISystem can be Burst compiled, but this is not yet the default.
// So we have to explicitly opt into Burst compilation with the [BurstCompile] attribute.
// It has to be added on BOTH the struct AND the OnCreate/OnDestroy/OnUpdate functions to be
// effective.
[BurstCompile]
partial struct SpawnerSystem : ISystem
{
    // A ComponentLookup provides random access to a component (looking up an entity).
    // We'll use it to extract the world space position and orientation of the spawn point (cannon nozzle).
    //ComponentLookup<LocalToWorldTransform> m_LocalToWorldTransformFromEntity;
    
    // Every function defined by ISystem has to be implemented even if empty.
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
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
        //Debug.Log("SpawnerSystem OnUpdate");
        
        float dt = SystemAPI.Time.DeltaTime;
        
        foreach (var unitSpawner in SystemAPI.Query<RefRW<UnitSpawnerComponent>>())
        {
            Debug.Log("Updating Unit Spawner");
            
            unitSpawner.ValueRW.counter += dt;
            
            if (unitSpawner.ValueRO.counter > unitSpawner.ValueRO.frequency)
            {
                //Registering jobs for the entities to spawn

                var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
                
                for (int i = 0; i < unitSpawner.ValueRO.max; i++)
                {
                    
                    var spawnJob = new SpawnUnit
                    {
                        //LocalToWorldTransformFromEntity = m_LocalToWorldTransformFromEntity,
                        ECB = ecb
                    };

                    // Schedule execution in a single thread, and do not block main thread.
                    spawnJob.Schedule();    
                    
                    
                    /*
                     * // ComponentLookup structures have to be updated every frame.
        m_LocalToWorldTransformFromEntity.Update(ref state);

        // Creating an EntityCommandBuffer to defer the structural changes required by instantiation.
        

        // Creating an instance of the job.
        // Passing it the ComponentLookup required to get the world transform of the spawn point.
        // And the entity command buffer the job can write to.
        var turretShootJob = new TurretShoot
        {
            LocalToWorldTransformFromEntity = m_LocalToWorldTransformFromEntity,
            ECB = ecb
        };

        // Schedule execution in a single thread, and do not block main thread.
        turretShootJob.Schedule();
                     */
                }

                unitSpawner.ValueRW.counter = 0.0f;
            }
        }
    }
}

// Requiring the Shooting tag component effectively prevents this job from running
// for the tanks which are in the safe zone.
//[WithAll(typeof(Shooting))]
[BurstCompile]
partial struct SpawnUnit : IJobEntity
{
    //[ReadOnly] public ComponentLookup<LocalToWorldTransform> LocalToWorldTransformFromEntity;
    public EntityCommandBuffer ECB;

    void Execute(in UnitSpawnerComponent unit)
    {
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