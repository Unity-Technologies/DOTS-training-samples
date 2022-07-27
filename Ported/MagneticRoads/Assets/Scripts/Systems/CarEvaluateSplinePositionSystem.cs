using Aspects;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    [WithAll(typeof(Car))]
    [BurstCompile]
    public partial struct CarEvaluateSplinePositionJob : IJobEntity
    {
        [ReadOnly]
        public ComponentDataFromEntity<RoadSegment> RoadSegmentFromEntity;
        public float DT;

        void Execute(ref CarAspect carAspect)
        {
            RoadSegmentFromEntity.TryGetComponent(carAspect.RoadSegment, out RoadSegment rs);
            carAspect.T = math.clamp(carAspect.T + ((carAspect.Speed * DT)), 0, 1);
            
            var anchor1 = rs.Start.Position + rs.Start.Tangent;
            var anchor2 = rs.End.Position - rs.End.Tangent;
            carAspect.Position = rs.Start.Position * (1f - carAspect.T) * (1f - carAspect.T) * (1f - carAspect.T) + 3f * anchor1 * (1f - carAspect.T) * (1f - carAspect.T) * carAspect.T + 3f * anchor2 * (1f - carAspect.T) * carAspect.T * carAspect.T + rs.End.Position * carAspect.T * carAspect.T * carAspect.T;
        }
    }

    [UpdateAfter(typeof(CarSpeedSystem))]
    [BurstCompile]
    partial struct CarEvaluateSplinePositionSystem : ISystem
    {
        ComponentDataFromEntity<RoadSegment> m_RoadSegmentFromEntity;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_RoadSegmentFromEntity = state.GetComponentDataFromEntity<RoadSegment>(true);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            m_RoadSegmentFromEntity.Update(ref state);

            var dt = state.Time.DeltaTime;

            var evaluatePositionOnSpline = new CarEvaluateSplinePositionJob
            {
                DT = dt,
                RoadSegmentFromEntity = m_RoadSegmentFromEntity
            };
            evaluatePositionOnSpline.ScheduleParallel();
        }
    }
}
