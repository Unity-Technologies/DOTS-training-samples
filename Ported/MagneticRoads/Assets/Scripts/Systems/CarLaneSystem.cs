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

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // This can probably become a parrelel job
            foreach (var buffer in SystemAPI.Query<DynamicBuffer<CarDynamicBuffer>>())
            {
                var entities = buffer.AsNativeArray().Reinterpret<Entity>();
                
                // Sort entities
                for (int x = 0; x < entities.Length; x++)
                {
                    var xPositionOnSpline = m_CarDataFromEntity[entities[x]].T;

                    for (int y = 0; y < entities.Length; y++)
                    {
                        if (x == y)
                            continue;
                        
                        var yPositionOnSpline = m_CarDataFromEntity[entities[y]].T;
                        
                        var xCarComponent = m_CarDataFromEntity[entities[x]];
                        var yCarComponent = m_CarDataFromEntity[entities[y]];
                        
                        if (math.abs(math.distance(xPositionOnSpline, yPositionOnSpline)) < 0.1f)
                        {
                            if (xPositionOnSpline - yPositionOnSpline > 0)
                            {
                                xCarComponent.Speed = 0;
                                ecb.SetComponent(entities[x], xCarComponent);
                            }
                            else
                            {
                                yCarComponent.Speed = 0;
                                ecb.SetComponent(entities[y], yCarComponent);
                            }
                        }
                        else
                        {
                            xCarComponent.Speed = 1f;
                            yCarComponent.Speed = 1f;
                            
                            ecb.SetComponent(entities[x], xCarComponent);
                            ecb.SetComponent(entities[y], yCarComponent);
                        }
                    }
                }
            }
        }
    }
}
