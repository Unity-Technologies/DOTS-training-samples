using Unity.Entities;

[GenerateAuthoringComponent]
public struct TrackPosition : IComponentData
{
    public float TrackProgress;
    public float Lane;

    public static float GetLoopedDistanceInFront(float ownProgress, float frontProgress, float trackLength)
    {
        if (frontProgress >= ownProgress)
        {
            return frontProgress - ownProgress;
        }
        else
        {
            return (trackLength - ownProgress) + frontProgress;
        }
    }
}
