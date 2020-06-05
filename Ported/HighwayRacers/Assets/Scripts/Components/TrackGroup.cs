using Unity.Entities;
using Unity.Mathematics;

public struct TrackGroup : ISharedComponentData
{
    public int Index;

    static public int LaneValueToTrackGroupIdx(float lane)
    {
        var laneBelow = (int)lane;
        var remainder = lane - laneBelow;
        var stepOffset = (int)math.floor(1.25f * remainder + 0.88f);
        return laneBelow * 2 + stepOffset;

        //if (lane - laneBelow < 0.1)
        //{
        //    return laneBelow * 2;
        //}
        //else if (lane - laneBelow > 0.9)
        //{
        //    return laneBelow * 2 + 2;
        //}
        //else
        //{
        //    return laneBelow * 2 + 1;
        //}
    }
}