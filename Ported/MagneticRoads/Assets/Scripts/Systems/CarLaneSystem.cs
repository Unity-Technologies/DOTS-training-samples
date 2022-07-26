using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
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
                        
                        var iCarComponent = m_CarDataFromEntity[entities[j]];
                        var jCarComponent = m_CarDataFromEntity[entities[j]];
                        
                        if (math.abs(math.distance(xPositionOnSpline, yPositionOnSpline)) < config.BrakingDistanceThreshold)
                        {
                            if (xPositionOnSpline - yPositionOnSpline > 0)
                            {
                                iCarComponent.Speed = 0;
                                ecb.SetComponent(entities[i], iCarComponent);
                            }
                            else
                            {
                                jCarComponent.Speed = 0;
                                ecb.SetComponent(entities[i], jCarComponent);
                            }
                        }
                        else
                        {
                            iCarComponent.Speed = 1f;
                            jCarComponent.Speed = 1f;
                            
                            ecb.SetComponent(entities[i], iCarComponent);
                            ecb.SetComponent(entities[i], jCarComponent);
                        }
                    }
                }
            }
        }
    }
}
