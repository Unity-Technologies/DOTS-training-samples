using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TrackPosition : IComponentData
{
    public float TrackProgress;
    public float Lane;

    public static float GetLoopedDistanceInFront(float ownProgress, float frontProgress, float trackLength)
    {
        return math.step(ownProgress, frontProgress) * (frontProgress - ownProgress) 
            + math.step(frontProgress, ownProgress) * ((trackLength - ownProgress) + frontProgress);
    }
}
