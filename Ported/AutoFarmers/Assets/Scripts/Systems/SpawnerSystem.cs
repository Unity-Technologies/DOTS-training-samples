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
        
        var random = new Unity.Mathematics.Random(51212);

        var data = GetSingleton<CommonData>();
        var settings = GetSingleton<CommonSettings>();
        
        Entities
            .WithAll<ProcessStoreSale>()
            .ForEach((Entity entity, in Translation translation) =>
            {
                ecb.RemoveComponent<ProcessStoreSale>(entity);

                // Add money.
                data.FarmerMoney++;
                data.DroneMoney++;
                
                // Spawn farmers if within budget.
                if (data.FarmerMoney >= settings.FarmerCost &&
                    data.FarmerCounter < settings.MaxFarmers)
                {
                    AddFarmer(ecb, settings.FarmerPrefab, translation.Value);
                    
                    data.FarmerMoney -= settings.FarmerCost;
                    data.FarmerCounter++;
                }
                
                // Spawn drones if within budget.
                if (data.DroneMoney >= settings.DroneCost)
                {
                    for (int i = 0; i < 5 && data.DroneCounter < settings.MaxDrones; i++)
                    {
                        AddDrone(ecb, settings.DronePrefab, translation.Value, random.NextFloat(2, 3));
                        data.DroneCounter++;
                    }
                    data.DroneMoney -= settings.DroneCost;
                }
            }).Run();

        SetSingleton(data);
        
        ecb.Playback(EntityManager);
    }

    private static Entity Add(EntityCommandBuffer ecb, Entity prefab, float3 position)
    {
        var instance = ecb.Instantiate(prefab);
        
        var translation = new Translation { Value = new float3(position) };
        ecb.SetComponent(instance, translation);

        return instance;
    }

    public static Entity AddFarmer(EntityCommandBuffer ecb, Entity prefab, float3 position)
    {
        var instance = Add(ecb, prefab, position);
        ecb.AddComponent(instance, new Farmer());
        ecb.AddComponent(instance, new Velocity());
        ecb.AddBuffer<PathNode>(instance);
        return instance;
    }

    public static Entity AddDrone(EntityCommandBuffer ecb, Entity prefab, float3 position, float hoverHeight)
    {
        var instance = ecb.Instantiate(prefab);
        
        ecb.AddComponent(instance, new Drone
        {
            // smoothPosition = position,
            // hoverHeight = m_Random.NextFloat(2, 3),
            // storePosition = new int2(x, y),
            // moveSmooth = data.MoveSmoothForDrones
        });
        
        ecb.AddComponent(instance, new DroneCheckPoints
        {
            hoverHeight = hoverHeight,
            storePosition = new int2((int)math.floor(position.x), (int)math.floor(position.z)),
        });
        
        ecb.AddComponent(instance, new Velocity());
        ecb.AddBuffer<PathNode>(instance);
        return instance;
    }
}