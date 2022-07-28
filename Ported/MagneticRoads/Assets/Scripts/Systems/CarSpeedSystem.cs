using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Systems
{
    [BurstCompile]
    partial struct LaneTrafficJob : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentDataFromEntity<Car> CarDataFromEntity;
        [ReadOnly] public ComponentDataFromEntity<RoadSegment> RoadSegmentDataFromEntity;
        public float MaxSpeed;
        public float BrakingDistance;
        
        public void Execute([ChunkIndexInQuery] int chunkIndex, in DynamicBuffer<CarDynamicBuffer> buffer)
        {
            var entities = buffer.AsNativeArray().Reinterpret<Entity>();

            for (int x = 0; x < entities.Length; x++)
            {
                var carX = CarDataFromEntity[entities[x]];
                var road = RoadSegmentDataFromEntity[carX.RoadSegment];
                    
                var tValueX = CarDataFromEntity[entities[x]].T;
                carX.Speed = MaxSpeed;

                for (int y = 0; y < entities.Length; y++)
                {
                    var tValueY = CarDataFromEntity[entities[y]].T;

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

                CarDataFromEntity[entities[x]] = carX;
            }
        }
    }
    
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

            
            var trafficJob = new LaneTrafficJob
            {
                CarDataFromEntity = m_CarDataFromEntity,
                RoadSegmentDataFromEntity = m_RoadSegmentDataFromEntity,
                MaxSpeed = MaxSpeed,
                BrakingDistance = BrakingDistance
            };
            trafficJob.ScheduleParallel();
        }
    }
}
