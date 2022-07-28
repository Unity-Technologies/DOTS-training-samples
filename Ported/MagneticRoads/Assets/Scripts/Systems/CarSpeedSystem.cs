using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    [UpdateAfter(typeof(TestSplineSpawnerSystem))]
    [BurstCompile]
    partial struct CarSpeedSystem : ISystem
    {
        public const float MaxSpeed = 3f;
        public const float BrakingDistance = 1.5f;

        ComponentDataFromEntity<Car> m_CarDataFromEntity;
        ComponentDataFromEntity<RoadSegment> m_RoadSegmentDataFromEntity;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_CarDataFromEntity = state.GetComponentDataFromEntity<Car>();
            m_RoadSegmentDataFromEntity = state.GetComponentDataFromEntity<RoadSegment>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            m_CarDataFromEntity.Update(ref state);
            m_RoadSegmentDataFromEntity.Update(ref state);

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // This can probably become a parrelel job
            foreach (var buffer in SystemAPI.Query<DynamicBuffer<CarDynamicBuffer>>())
            {
                var entities = buffer.AsNativeArray().Reinterpret<Entity>();

                // Sort entities
                for (int x = 0; x < entities.Length; x++)
                {
                    var carX = m_CarDataFromEntity[entities[x]];
                    var road = m_RoadSegmentDataFromEntity[carX.RoadSegment];
                    
                    var tValueX = m_CarDataFromEntity[entities[x]].T;
                    carX.Speed = MaxSpeed;

                    for (int y = 0; y < entities.Length; y++)
                    {
                        var tValueY = m_CarDataFromEntity[entities[y]].T;

                        // X is behind and must brake
                        if (tValueY > tValueX)
                        {
                            var distance = (tValueY - tValueX) * road.Length;
                            if (distance < BrakingDistance)
                            {
                                carX.Speed = 0;
                            }
                        }
                    }

                    ecb.SetComponent(entities[x], carX);
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
