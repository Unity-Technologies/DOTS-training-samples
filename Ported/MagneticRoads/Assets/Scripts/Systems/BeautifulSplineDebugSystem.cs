using Components;
using Unity.Burst;
using Unity.Entities;
using Util;
using Unity.Mathematics;

namespace Systems
{
    [BurstCompile]
    partial struct BeautifulSplineDebugSystem : ISystem
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
        }
    }
}