using System.Text.RegularExpressions;
using Aspects;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [BurstCompile]
    partial struct CarLaneSystem : ISystem
    {
        ComponentDataFromEntity<Car> m_CarDataFromEntity;

        public void OnCreate(ref SystemState state)
        {
            m_CarDataFromEntity = state.GetComponentDataFromEntity<Car>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            m_CarDataFromEntity.Update(ref state);

            var config = SystemAPI.GetSingleton<Config>();
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // This can probably become a parrelel job
            foreach (var buffer in SystemAPI.Query<DynamicBuffer<CarDynamicBuffer>>())
            {
                var entities = buffer.AsNativeArray().Reinterpret<Entity>();
                
                // Sort entities
                for (int i = 0; i < entities.Length; i++)
                {
                    var xPositionOnSpline = m_CarDataFromEntity[entities[i]].T;

                    for (int j = 0; j < entities.Length; j++)
                    {
                        if (i == j)
                            continue;
                        
                        var yPositionOnSpline = m_CarDataFromEntity[entities[j]].T;
                        
                        if (math.abs(math.distance(xPositionOnSpline, yPositionOnSpline)) < config.BrakingDistanceThreshold)
                        {
                            if (xPositionOnSpline - yPositionOnSpline > 0)
                            {
                                ecb.SetComponentEnabled<Braking>(entities[i], true);
                            }
                            else
                            {
                                // y in front of x
                                ecb.SetComponentEnabled<Braking>(entities[j], true);
                            }
                        }
                        else
                        {
                            ecb.SetComponentEnabled<Braking>(entities[i], false);
                            ecb.SetComponentEnabled<Braking>(entities[j], false);
                        }
                    }
                }
            }
        }
    }
}
