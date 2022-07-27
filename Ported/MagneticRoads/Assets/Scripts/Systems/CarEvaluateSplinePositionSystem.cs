using Aspects;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Util;

namespace Systems
{
    [WithAll(typeof(Car))]
    [BurstCompile]
    partial struct CarEvaluateSplinePositionJob : IJobEntity
    {
        [ReadOnly] public ComponentDataFromEntity<RoadSegment> RoadSegmentFromEntity;
        public float DT;

        void Execute(ref CarAspect carAspect)
        {
            carAspect.T = math.clamp(carAspect.T + (carAspect.Speed * DT), 0, 1);
            RoadSegmentFromEntity.TryGetComponent(carAspect.RoadSegment, out RoadSegment rs);

            carAspect.Position = Spline.Evaluate(rs.Start, rs.End, carAspect.T);
        }
    }

    [BurstCompile]
    partial struct CarTravelSystem : ISystem
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
