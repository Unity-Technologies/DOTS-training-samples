using Unity.Entities;
using Unity.Mathematics;

public struct SettingsBlobAsset
{
    public float MaxSpeed;
    public float CarriageSizeWithMargins;
    public float TimeAtStation;
    public float BreakingDistance;
    public float Acceleration;
    public BlobArray<float4> LineColors;
}


