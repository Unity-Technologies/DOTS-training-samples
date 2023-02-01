using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

[BurstCompile]
partial struct HumanRoutingSystem : ISystem
{
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //state.Enabled = false;
        //var config = SystemAPI.GetSingleton<Config>();


        List<(Entity, LocalTransform)> humansWaitingForRoute = new List<(Entity,LocalTransform)>();
        //Check for people waiting to get a route
        foreach (var (human, _, humanTransform, ent) in SystemAPI.Query<RefRW<Human>, RefRW<HumanWaitForRouteTag>, RefRO<LocalTransform>>().WithEntityAccess())
        {
            humansWaitingForRoute.Add((ent, humanTransform.ValueRO));
        }

        foreach (var (human, humanTransform) in humansWaitingForRoute)
        {
            Debug.Log("Human with tag found");
            Station nearestStation = new Station();
            float shortestDistance = float.MaxValue;
            
            foreach (var (station, stationTransform) in SystemAPI.Query<RefRO<Station>, RefRO<LocalTransform>>())
            {
                var distance = Vector3.Distance(humanTransform.Position, stationTransform.ValueRO.Position);

                if (distance <= shortestDistance)
                {
                    shortestDistance = distance;
                    nearestStation = station.ValueRO;
                }
            }
            // Nusprest ar Queue ar PerTilta
            if (Random.Range(1, 1).Equals(1))
            {
                //Queue picked
                var distance1 = Mathf.Abs(humanTransform.Position.y - nearestStation.StationWaypoints.QueuePoints1[0].y);
                var distance2 = Mathf.Abs(humanTransform.Position.y - nearestStation.StationWaypoints.QueuePoints2[0].y);

                if (distance1 < distance2)
                {
                    state.EntityManager.SetComponentData(human, new HumanRoute {BridgeRoute = new NativeList<float3>(),
                        QueuePoint = nearestStation.StationWaypoints.QueuePoints1[Random.Range(0, nearestStation.StationWaypoints.QueuePoints1.Length - 1)]});
                }
                else
                {
                    state.EntityManager.SetComponentData(human, new HumanRoute {BridgeRoute = new NativeList<float3>(),
                        QueuePoint = nearestStation.StationWaypoints.QueuePoints2[Random.Range(0, nearestStation.StationWaypoints.QueuePoints2.Length - 1)]});
                }
            }
            else
            {
                //Bridge picked
                
            }
            //nearestStation.StationWaypoints.QueuePoints1

            state.EntityManager.RemoveComponent<HumanWaitForRouteTag>(human);
        }
    }
}
