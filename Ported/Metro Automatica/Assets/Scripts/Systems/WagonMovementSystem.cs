using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UI;

[BurstCompile]
partial struct WagonMovementSystem : ISystem
{
    private float startTime;
    public float speed;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        startTime = Time.time;
        speed = 0.01f;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();


        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach (var (wagon, transform, ent) in SystemAPI.Query<RefRW<Wagon>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            var stationList = SystemAPI.GetBuffer<StationWayPoints>(ent);

            float3 finalDestination = stationList[wagon.ValueRO.StationCounter + wagon.ValueRO.Direction].Value;
            float distCovered = (Time.time - startTime) * speed;
            
            // Fraction of journey completed equals current distance divided by total distance.
            float journeyLength = Vector3.Distance(transform.ValueRO.Position, finalDestination);
            float fractionOfJourney = distCovered / journeyLength;
            transform.ValueRW.Position = Vector3.Lerp(transform.ValueRO.Position, finalDestination, fractionOfJourney);

            
            
            
            if (journeyLength < 0.001f)
            {
                if(wagon.ValueRW.StationCounter == 0 && wagon.ValueRW.Direction > 0)
                    wagon.ValueRW.StationCounter += wagon.ValueRO.Direction;
                if (wagon.ValueRO.StationCounter == stationList.Length - 1)
                {
                    wagon.ValueRW.Direction *= -1;
                }
                
            }
        }
    }
}
