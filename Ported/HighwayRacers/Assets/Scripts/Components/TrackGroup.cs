using Unity.Entities;

public struct TrackGroup : ISharedComponentData
{
    public int Index;

    static public int LaneValueToTrackGroupIdx(float lane)
    {
        int laneBelow = (int)lane;

        if (lane - laneBelow < 0.1)
        {
            return laneBelow * 2;
        }
        else if (lane - laneBelow > 0.9)
        {
            return laneBelow * 2 + 2;
        }
        else
        {
            return laneBelow * 2 + 1;
        }
    }
}