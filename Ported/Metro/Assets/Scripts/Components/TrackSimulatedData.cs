using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TrackSimulatedData : IComponentData
{
    public float TotalLength;
    public float3 ControlPoint0;
    public float3 ControlPoint1;
    public float3 ControlPoint2;
    public float3 ControlPoint3;
    public float SegmentLength0;
    public float SegmentLength1;
    public float SegmentLength2;
    public float SegmentLength3;

    public float3 GetPositionForProgress(float progress)
    {
        if (progress<SegmentLength0)
        {
            return ControlPoint0 + (ControlPoint1 - ControlPoint0) * (progress / SegmentLength0);
        }
        if (progress<SegmentLength1)
        {
            return ControlPoint1 + (ControlPoint2 - ControlPoint1) * (progress-SegmentLength0) / (SegmentLength1-SegmentLength0);
        }
        if (progress<SegmentLength2)
        {
            return ControlPoint2 + (ControlPoint3 - ControlPoint2) * (progress-SegmentLength1) / (SegmentLength2-SegmentLength1);
        }
        if (progress<SegmentLength3)
        {
            return ControlPoint3 + (ControlPoint0 - ControlPoint3) * (progress-SegmentLength2) / (SegmentLength3-SegmentLength2);
        }
        return 0;
    }

    public quaternion GetQuaternionForProgress(float progress)
    {
        if (progress<SegmentLength0)
        {
            return quaternion.LookRotation(ControlPoint1-ControlPoint0, new float3(0, 1, 0));
        }
        if (progress<SegmentLength1)
        {
            return quaternion.LookRotation(ControlPoint2-ControlPoint1, new float3(0, 1, 0));
        }
        if (progress<SegmentLength2)
        {
            return quaternion.LookRotation(ControlPoint3-ControlPoint2, new float3(0, 1, 0));
        }
        if (progress<SegmentLength3)
        {
            return quaternion.LookRotation(ControlPoint0-ControlPoint3, new float3(0, 1, 0));
        }
        
        return quaternion.identity;
    }
}
