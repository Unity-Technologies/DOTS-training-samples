using Components;
using Unity.Burst;
using Unity.Entities;
using Util;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    partial struct BeautifulWorldDebugSystem : ISystem
    {
        public const int DebugSplineResolution = 50;

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
            foreach (var roadSegment in SystemAPI.Query<RefRO<RoadSegment>>())
            {
                var rs = roadSegment.ValueRO;
                float3 prevPos = Spline.EvaluatePosition(rs.Start, rs.End, 0);
                float3 nextPos;

                for (int i = 0; i < DebugSplineResolution; i++)
                {
                    var nextT = ((float)i + 1) / DebugSplineResolution;
                    nextPos = Spline.EvaluatePosition(rs.Start, rs.End, nextT);

                    UnityEngine.Debug.DrawLine(prevPos, nextPos);

                    prevPos = nextPos;
                }
            }

            foreach (var intersection in SystemAPI.Query<TransformAspect>().WithAll<Intersection>())
            {
                var pos = intersection.Position;
                var fwd2 = intersection.Forward * 2;
                var right2 = intersection.Right * 2;
                // Box
                UnityEngine.Debug.DrawLine(pos + fwd2 - right2, pos + fwd2 + right2);
                UnityEngine.Debug.DrawLine(pos + fwd2 + right2, pos - fwd2 + right2);
                UnityEngine.Debug.DrawLine(pos - fwd2 + right2, pos - fwd2 - right2);
                UnityEngine.Debug.DrawLine(pos - fwd2 - right2, pos + fwd2 - right2);
                // Cross
                UnityEngine.Debug.DrawLine(pos + fwd2, pos - fwd2);
                UnityEngine.Debug.DrawLine(pos + right2, pos - right2);
                
            }
        }
    }
}