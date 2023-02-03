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
    private EntityQuery humanQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        humanQuery = SystemAPI.QueryBuilder().WithAll<Human, HumanWaitForRouteTag, LocalTransform>().Build();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var humanTransforms = humanQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        var humanData = humanQuery.ToComponentDataArray<Human>(Allocator.Temp);
        var humanEntities = humanQuery.ToEntityArray(Allocator.Temp);

        for (int i = 0; i < humanEntities.Length; i++)
        {
            Entity nearestStation = new Entity();
            float shortestDistance = float.MaxValue;

            var humanTransform = humanTransforms[i];
            foreach (var (station, stationTransform, stationEntity) in SystemAPI.Query<RefRO<Station>, RefRO<LocalTransform>>().WithEntityAccess())
            {
                var distance = Vector3.Distance(humanTransform.Position, stationTransform.ValueRO.Position);

                if (distance <= shortestDistance)
                {
                    shortestDistance = distance;
                    nearestStation = stationEntity;
                }
            }
            if (Random.Range(1, 1).Equals(1))
            {
                var queuePointsColl = SystemAPI.GetBuffer<QueueWaypointCollection>(nearestStation);
                var bridgePointsColl = SystemAPI.GetBuffer<BridgeWaypointCollection>(nearestStation);
                //Queue picked
                var distance1 = Vector3.Distance(humanTransform.Position, queuePointsColl[0].North);
                var distance2 = Vector3.Distance(humanTransform.Position, queuePointsColl[0].South);

                var tempHuman = humanData[i];
                
                if (distance1 < distance2)
                {
                    tempHuman.QueuePoint = queuePointsColl[Random.Range(0, queuePointsColl.Length - 1)].North;
                    state.EntityManager.SetComponentData(humanEntities[i], tempHuman);
                }
                else
                {
                    tempHuman.QueuePoint = queuePointsColl[Random.Range(0, queuePointsColl.Length - 1)].South;
                    state.EntityManager.SetComponentData(humanEntities[i], tempHuman);
                }
            }
            else
            {
                //Bridge picked
                
            }

            state.EntityManager.RemoveComponent<HumanWaitForRouteTag>(humanEntities[i]);
        }
    }
}
