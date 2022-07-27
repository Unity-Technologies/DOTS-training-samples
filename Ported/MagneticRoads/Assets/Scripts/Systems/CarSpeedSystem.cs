using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(TestSplineSpawnerSystem))]
    [BurstCompile]
    partial struct CarSpeedSystem : ISystem
    {
        ComponentDataFromEntity<Car> m_CarDataFromEntity;

        [BurstCompile]
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

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // This can probably become a parrelel job
            foreach (var buffer in SystemAPI.Query<DynamicBuffer<CarDynamicBuffer>>())
            {
                var entities = buffer.AsNativeArray().Reinterpret<Entity>();

                // Sort entities
                for (int x = 0; x < entities.Length; x++)
                {
                    var xPositionOnSpline = m_CarDataFromEntity[entities[x]].T;
                    var xCarComponent = m_CarDataFromEntity[entities[x]];
                    xCarComponent.Speed = 3f;

                    for (int y = 0; y < entities.Length; y++)
                    {
                        if (x == y)
                            continue;

                        var yPositionOnSpline = m_CarDataFromEntity[entities[y]].T;

                        if (math.distance(xPositionOnSpline, yPositionOnSpline) < 0.4f)
                        {
                            if (yPositionOnSpline - xPositionOnSpline > 0)
                            {
                                xCarComponent.Speed = 0;
                            }
                        }
                    }

                    ecb.SetComponent(entities[x], xCarComponent);
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
