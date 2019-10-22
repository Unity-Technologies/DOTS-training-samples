using Unity.Entities;

struct CarStateOnTrack : IComponentData
{
    public int TrackSegment;
    public float PositionOnTrackSegmentLane0;
    public float Lane;
    public float TargetLane;
    public float TimeOvertakeCarSet;
}
