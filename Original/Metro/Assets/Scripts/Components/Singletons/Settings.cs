using Unity.Entities;
using UnityMonoBehavior = UnityEngine.MonoBehaviour;

public struct Settings : IComponentData
{
    /// <summary>
    /// Max train speed in m/s
    /// </summary>
    public float MaxSpeed;
    
    public float CarriageSizeWithMargins;
    
    /// <summary>
    /// How long time in seconds do we wait at station?
    /// </summary>
    public float TimeAtStation;

    /// <summary>
    /// How far from the station does the train start breaking (in meters)
    /// </summary>
    public float PlatformBrakingDistance;

    /// <summary>
    /// Acceleration in m/s^2
    /// </summary>
    public float Acceleration;

    /// <summary>
    /// When do we initiate braking when approaching train in front
    /// </summary>
    public float TrainBrakingDistance;

    /// <summary>
    /// If we're leaving platform, keep running if we're within this margin
    /// </summary>
    public float LeavingPlatformMarginDistance;

    /// <summary>
    /// How far from train in front should we stop?
    /// </summary>
    public float MarginToTrainInFrontDistance;

    public BlobAssetReference<SettingsBlobAsset> SettingsBlobRef;
}
