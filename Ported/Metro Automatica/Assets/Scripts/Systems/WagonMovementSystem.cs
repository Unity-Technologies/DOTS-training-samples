using System.Linq;
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
       // var config = SystemAPI.GetSingleton<Config>();


       // var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
       // var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
       /* foreach (var (wagon, transform, stationList) in SystemAPI.Query<RefRW<Wagon>, RefRW<LocalTransform>, DynamicBuffer<StationWayPoints>>())
        {
            float3 finalDestination = stationList[wagon.ValueRO.StationCounter].Value;
            float distCovered = (Time.time - startTime) * speed;
            float journeyLength = Vector3.Distance(transform.ValueRO.Position, finalDestination);
            float fractionOfJourney = distCovered / journeyLength;
            transform.ValueRW.Position = Vector3.Lerp(transform.ValueRO.Position, finalDestination, fractionOfJourney);
            
            if (journeyLength < 0.001f)
            {
                if (wagon.ValueRO.StationCounter == 0 && wagon.ValueRO.Direction > 0)
                {
                    wagon.ValueRW.StationCounter += wagon.ValueRO.Direction;
                    Debug.Log($"Sending to next station");
                }
                else if(wagon.ValueRO.StationCounter > 0 && wagon.ValueRO.StationCounter < stationList.Length - 1)
                {
                    wagon.ValueRW.StationCounter += wagon.ValueRO.Direction;
                    Debug.Log($"2");
                }
                else if (wagon.ValueRO.StationCounter == stationList.Length - 1)
                {
                    Debug.Log($"3");
                    wagon.ValueRW.Direction = -1;
                    wagon.ValueRW.StationCounter += wagon.ValueRO.Direction;
                }
                else if(wagon.ValueRO.StationCounter == 0 && wagon.ValueRO.Direction < 0)
                {
                    wagon.ValueRW.Direction = 1;
                    wagon.ValueRW.StationCounter += wagon.ValueRO.Direction;
                    Debug.Log($"Going back");
                }
            }
        }*/
       var jobExecute = new WagonMovementJob();
       jobExecute.elapsedTime = (float)SystemAPI.Time.ElapsedTime;
       jobExecute.ScheduleParallel();
    }
}

[BurstCompile]
partial struct WagonMovementJob : IJobEntity
{
    public float elapsedTime;
    public void Execute(ref Wagon wagon, ref LocalTransform transform, DynamicBuffer<StationWayPoints> stationList)
    {
        const float speed = 0.01f;
        float3 finalDestination = stationList[wagon.StationCounter].Value;
        float distCovered = (elapsedTime) * speed;
        float journeyLength = Vector3.Distance(transform.Position, finalDestination);
        float fractionOfJourney = distCovered / journeyLength;
        transform.Position = Vector3.Lerp(transform.Position, finalDestination, fractionOfJourney);
            
        if (journeyLength < 0.001f)
        {
            if (wagon.StationCounter == 0 && wagon.Direction > 0)
            {
                wagon.StationCounter += wagon.Direction;
                Debug.Log($"Sending to next station");
            }
            else if(wagon.StationCounter > 0 && wagon.StationCounter < stationList.Length - 1)
            {
                wagon.StationCounter += wagon.Direction;
                Debug.Log($"2");
            }
            else if (wagon.StationCounter == stationList.Length - 1)
            {
                Debug.Log($"3");
                wagon.Direction = -1;
                wagon.StationCounter += wagon.Direction;
            }
            else if(wagon.StationCounter == 0 && wagon.Direction < 0)
            {
                wagon.Direction = 1;
                wagon.StationCounter += wagon.Direction;
                Debug.Log($"Going back");
            }
        }
    }
}
