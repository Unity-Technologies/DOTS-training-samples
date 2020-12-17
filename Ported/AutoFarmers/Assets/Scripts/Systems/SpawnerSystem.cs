using Unity.Burst.Intrinsics;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(FarmInitializeSystem))]
public class SpawnerSystem : SystemBase
{
    private const int k_PlantsToSpawnfarmer = 10;
    private const int k_PlantsToSpawnDrone = 50;
    
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<CommonData>();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var data = GetSingleton<CommonData>();
        var settings = GetSingleton<CommonSettings>();

        // Farmer spawning rule:
        // When enough resources have been collected into a silo, a new farmer spawns from the silo.
        Entities
            .WithAll<Store>()
            .ForEach((Entity entity, ref StoreResourceCount resourceCount, in Translation translation) =>
            {
                // 1) If the silo's resource counter is beyond the threshold
                if (resourceCount.Value >= 5)
                {
                    // 2) Reset the silo's counter
                    resourceCount.Value = 0;
                    
                    // 3) Add a drone on the silo location for every Nth farmer.
                    if (data.FarmerCount % 5 == 0)
                    {
                        data.DroneCount++;
                        AddDrone(ecb, settings.DronePrefab, new int3(translation.Value));
                    }
                    // 4) Otherwise add a farmer.
                    else
                    {
                        data.FarmerCount++;
                        AddFarmer(ecb, settings.FarmerPrefab, new int3(translation.Value));
                    }
                }
            }).Run(); // TODO: Parallel

        SetSingleton(data);
        ecb.Playback(EntityManager);
    }

    private static Entity Add(EntityCommandBuffer ecb, Entity prefab, int3 position)
    {
        var instance = ecb.Instantiate(prefab);
        
        var translation = new Translation { Value = new float3(position) };
        ecb.SetComponent(instance, translation);

        return instance;
    }

    public static Entity AddFarmer(EntityCommandBuffer ecb, Entity prefab, int3 position)
    {
        var instance = Add(ecb, prefab, position);
        ecb.AddComponent(instance, new Farmer());
        ecb.AddComponent(instance, new Velocity());
        ecb.AddBuffer<PathNode>(instance);
        return instance;
    }

    public static Entity AddDrone(EntityCommandBuffer ecb, Entity prefab, int3 position)
    {
        var instance = Add(ecb, prefab, position);
        ecb.AddComponent(instance, new Drone());
        ecb.AddComponent(instance, new Velocity());
        ecb.AddBuffer<PathNode>(instance);
        return instance;
    }
}