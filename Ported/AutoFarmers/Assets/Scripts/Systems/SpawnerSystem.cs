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

    private Unity.Mathematics.Random m_Random;
    
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<CommonData>();
        m_Random = new Unity.Mathematics.Random(51212);
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

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
                if (data.DroneMoney >= settings.DroneCost &&
                    data.DroneCounter < settings.MaxDrones)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        var position = new float3(translation.Value.x + .5f, 0f, translation.Value.z + .5f);
                        
                        AddDrone(ecb, settings.DronePrefab, position);
                        data.DroneCounter++;
                    }
                    data.DroneMoney -= settings.DroneCost;
                }
            }).Run(); // TODO: Parallel

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

    public static Entity AddDrone(EntityCommandBuffer ecb, Entity prefab, float3 position)
    {
        var instance = ecb.Instantiate(prefab);
        
        ecb.AddComponent(instance, new Drone
        {
            // smoothPosition = position,
            // hoverHeight = m_Random.NextFloat(2, 3),
            // storePosition = new int2(x, y),
            // moveSmooth = data.MoveSmoothForDrones
        });
        
        ecb.AddComponent(instance, new Velocity());
        ecb.AddBuffer<PathNode>(instance);
        return instance;
    }
}