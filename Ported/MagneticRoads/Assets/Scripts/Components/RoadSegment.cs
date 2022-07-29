using Unity.Collections;
using Unity.Entities;
using Util;

namespace Components
{
    public struct RoadSegment: IComponentData
    {
        [ReadOnly] public Spline.RoadTerminator Start;
        [ReadOnly] public Spline.RoadTerminator End;
        [ReadOnly] public float Length;

        [ReadOnly] public Entity StartIntersection;
        [ReadOnly] public Entity EndIntersection;
    }
}