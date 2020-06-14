using System.Threading;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace HighwayRacer
{
    [UpdateAfter(typeof(SegmentizeSys))]
    public class SetTransformSys : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            const float laneWidth = 1.8f; // todo verify value

            RoadInit road = GameObject.FindObjectOfType<RoadInit>();
            if (road != null)
            {
                var infos = road.segmentInfos;
                
                Entities.ForEach((ref Translation translation, ref Rotation rotation, in TrackSegment segment, in TrackPos pos, in Lane lane) =>
                {
                    var seg = infos[segment.Val];
                    var posInSegment = pos.Val - (seg.Threshold - seg.Length);
                    if (seg.Curved)
                    {
                        var percentage = posInSegment / seg.Length;

                        rotation.Value = new quaternion(math.lerp(seg.DirectionRot.value, seg.DirectionRotEnd.value, percentage));

                        var radius = seg.Radius + lane.Val * laneWidth;
                        float x, y;
                        switch (seg.Direction)
                        {
                            case Cardinal.UP:
                                // find x, y of quarter circle traced from (-r, 0) to (0, r)
                                x = math.lerp(-radius, 0, percentage); 
                                y = math.sqrt(radius * radius - x * x);
                                translation.Value = seg.Position;
                                translation.Value += seg.DirectionLaneOffset * lane.Val;
                                translation.Value += new float3(x + radius, 0, y);
                                break;
                            case Cardinal.DOWN:
                                // find x, y of quarter circle traced from (r, 0) to (0, -r)
                                x = math.lerp(radius, 0, percentage);
                                y = -math.sqrt(radius * radius - x * x);
                                translation.Value = seg.Position;
                                translation.Value += seg.DirectionLaneOffset * lane.Val;
                                translation.Value += new float3(x - radius, 0, y);
                                break;
                            case Cardinal.LEFT:
                                // find x, y of quarter circle traced from (0, -r) to (-r, 0)
                                x = math.lerp(0, -radius, percentage);
                                y = -math.sqrt(radius * radius - x * x);
                                translation.Value = seg.Position;
                                translation.Value += seg.DirectionLaneOffset * lane.Val;
                                translation.Value += new float3(x, 0, y + radius);
                                break;
                            case Cardinal.RIGHT:
                                // find x, y of quarter circle traced from (0, r) to (r, 0)
                                x = math.lerp(0, radius, percentage);
                                y = math.sqrt(radius * radius - x * x);
                                translation.Value = seg.Position;
                                translation.Value += seg.DirectionLaneOffset * lane.Val;
                                translation.Value += new float3(x, 0, y - radius);
                                break;
                        }
                    }
                    else
                    {
                        
                        translation.Value = seg.Position;
                        translation.Value += posInSegment * seg.DirectionVec;
                        translation.Value += seg.DirectionLaneOffset * lane.Val;
                        rotation.Value = seg.DirectionRot;
                    }
                }).ScheduleParallel();
            }
        }
    }
}