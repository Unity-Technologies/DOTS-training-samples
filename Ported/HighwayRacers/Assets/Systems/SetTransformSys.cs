using Unity.Entities;
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
                var thresholds = road.segmentThresholds;
                var infos = road.segmentInfos;
                
                Entities.ForEach((ref Translation translation, ref Rotation rotation, in TrackSegment segment, in TrackPos pos, in Lane lane) =>
                {
                    var posInSegment = thresholds[segment.Val] - pos.Val;
                    var info = infos[segment.Val];
                    if (info.curved)
                    {
                        var start = info.position;
                    }
                    else
                    {
                        var laneOffset = info.laneOffsetDir * lane.Val;
                        translation.Value = info.position + laneOffset + posInSegment * info.direction;
                    }
                }).Run();
            }
        }
    }
}