using System.Threading;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace HighwayRacer
{
    [UpdateAfter(typeof(AvoidanceMergeSpeedSys))]
    public class SetTransformAndColorSys : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public readonly static float3 cruiseColor = new float3(0.5f, 0.5f, 0.5f);
        public readonly static float3 fastestColor = new float3(0, 1.0f, 0);
        public readonly static float3 slowestColor = new float3(1.0f, 0.0f, 0);

        private static void setColor(in Car car, ref URPMaterialPropertyBaseColor color)
        {
            if (car.Speed >= car.DesiredSpeedUnblocked)
            {
                var percentage = (car.Speed - car.DesiredSpeedUnblocked) / (car.DesiredSpeedOvertake - car.DesiredSpeedUnblocked);
                color.Value = new float4(math.lerp(cruiseColor, fastestColor, percentage), 1.0f);
            }
            else
            {
                var percentage = (car.Speed - CarSpawnSys.minSpeed) / (car.DesiredSpeedUnblocked - CarSpawnSys.minSpeed);
                color.Value = new float4(math.lerp(slowestColor, cruiseColor, percentage), 1.0f);
            }
        }

        private static void setTransform(ref Car car, ref Translation translation, ref Rotation rotation, ref RoadSegment seg)
        {
            var laneOffsetDist = car.LaneOffsetDist();

            translation.Value = seg.Position;
            translation.Value += seg.DirectionLaneOffset * laneOffsetDist;

            if (seg.IsCurved())  
            {
                var percentage = car.Pos / seg.Length;
                rotation.Value = new quaternion(math.lerp(seg.DirectionRot.value, seg.DirectionRotEnd.value, percentage));
                var radius = seg.Radius + (laneOffsetDist * RoadSys.laneWidth); 

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
                var angle = math.lerp(math.radians(0), math.radians(-90), percentage); // -90 because cos & sin assume counter-clockwise
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
                translation.Value += car.Pos * seg.DirectionVec;
                rotation.Value = seg.DirectionRot;
            }
        }

        protected override void OnUpdate()
        {
            Car car;
            RoadSegment segment;
            var cars = new CarEnumerator(RoadSys.CarBuckets, RoadSys.roadSegments, out segment);

            // todo: to make parallel, ultimately need IJobChunk? per chunk, keep separate Car enumerator
            Entities.WithReadOnly(RoadSys.CarBuckets).WithReadOnly(RoadSys.roadSegments).ForEach(
                (ref Translation translation, ref Rotation rotation, ref URPMaterialPropertyBaseColor color) =>
                {
                    cars.Next(out car, ref segment);
                    setTransform(ref car, ref translation, ref rotation, ref segment);
                    setColor(in car, ref color);
                }).Run();
        }
    }
}