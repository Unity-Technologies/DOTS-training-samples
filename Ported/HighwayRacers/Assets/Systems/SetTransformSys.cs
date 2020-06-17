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

        private static void setTransform(in NativeArray<RoadSegment> infos, in TrackPos pos, in TrackSegment segment, float lane,
            ref Translation translation, ref Rotation rotation)
        {
            const float laneWidth = 1.8f; // todo: verify value

            var seg = infos[segment.Val];
            var posInSegment = pos.Val - (seg.Threshold - seg.Length);

            translation.Value = seg.Position;
            translation.Value += seg.DirectionLaneOffset * lane;

            if (seg.Curved)
            {
                var percentage = posInSegment / seg.Length;

                rotation.Value = new quaternion(math.lerp(seg.DirectionRot.value, seg.DirectionRotEnd.value, percentage));

                var radius = seg.Radius + lane * laneWidth; // todo: seg.Radius can just be const because will always be same 

                // rotate the origin around pivot to get displacement
                float3 pivot = new float3();
                float3 displacement = float3.zero;

                switch (seg.Direction)
                {
                    case Cardinal.UP:
                        pivot = new float3(radius, 0, 0);
                        break;
                    case Cardinal.DOWN:
                        pivot = new float3(-radius, 0, 0);
                        break;
                    case Cardinal.LEFT:
                        pivot = new float3(0, 0, radius);
                        break;
                    case Cardinal.RIGHT:
                        pivot = new float3(0, 0, -radius);
                        break;
                }

                // rotate displacement by angle around pivot
                var angle = math.lerp(math.radians(0), math.radians(-90), percentage); // -90 because cos & sin assume clockwise
                displacement -= pivot;
                var c = math.cos(angle);
                var s = math.sin(angle);
                float x = displacement.x * c - displacement.z * s;
                displacement.z = displacement.z * c + displacement.x * s;
                displacement.x = x;
                displacement += pivot;

                translation.Value += displacement;
            }
            else
            {
                translation.Value += posInSegment * seg.DirectionVec;
                rotation.Value = seg.DirectionRot;
            }
        }

        protected override void OnUpdate()
        {
            if (RoadInit.roadSegments.IsCreated)
            {
                var infos = RoadInit.roadSegments;

                Entities.WithNone<LaneOffset>().ForEach((ref Translation translation, ref Rotation rotation,
                    in TrackSegment segment, in TrackPos pos, in Lane lane) =>
                {
                    setTransform(in infos, in pos, in segment, lane.Val, ref translation, ref rotation);
                }).Run();

                Entities.ForEach((ref Translation translation, ref Rotation rotation, in TrackSegment segment, in TrackPos pos,
                    in Lane lane, in LaneOffset laneOffset) =>
                {
                    setTransform(in infos, in pos, in segment, lane.Val + laneOffset.Val, ref translation, ref rotation);
                }).Run();
            }
        }
    }
}