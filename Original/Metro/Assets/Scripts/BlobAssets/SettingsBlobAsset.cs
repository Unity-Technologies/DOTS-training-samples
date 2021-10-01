using Unity.Entities;
using Unity.Mathematics;

public struct SettingsBlobAsset
{
    public float MaxSpeed;
    public float CarriageSizeWithMargins;
    public float TimeAtStation;
    public float TrainBrakingDistance;
    public float PlatformBrakingDistance;
    public float Acceleration;
    public BlobArray<float4> LineColors;
    public float LeavingPlatformMarginDistance;
    public float MarginToTrainInFrontDistance;
}


