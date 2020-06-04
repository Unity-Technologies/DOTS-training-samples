using Unity.Entities;

public struct TrackGroup : ISharedComponentData
{
    public int index;

    public void SetTrack(float lane)
    {
        index = GetTrackGroupIdx(lane);
    }

    static public int GetTrackGroupIdx(float lane)
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