using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityMonoBehavior = UnityEngine.MonoBehaviour;

public class SettingsAuthoring : UnityMonoBehavior, IConvertGameObjectToEntity
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
    public float BreakingDistance;

    /// <summary>
    /// Acceleration in m/s^2
    /// </summary>
    public float Acceleration;

    public Color[] LineColors;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        using (var blobBuilder = new BlobBuilder(Allocator.Temp))
        {
            ref var settingsBlobAsset = ref blobBuilder.ConstructRoot<SettingsBlobAsset>();
            settingsBlobAsset.MaxSpeed = MaxSpeed;
            settingsBlobAsset.CarriageSizeWithMargins = CarriageSizeWithMargins;
            settingsBlobAsset.TimeAtStation = TimeAtStation;
            settingsBlobAsset.BreakingDistance = BreakingDistance;
            settingsBlobAsset.Acceleration = Acceleration;

            var settingsLineColors = blobBuilder.Allocate(ref settingsBlobAsset.LineColors, LineColors.Length);
            for (int i = 0; i < LineColors.Length; i++)
            {
                var lineColor = LineColors[i];
                settingsLineColors[i] = new float4(lineColor.r, lineColor.g,lineColor.b,lineColor.a);
            }

            BlobAssetReference<SettingsBlobAsset> blobAssetReference =
                blobBuilder.CreateBlobAssetReference<SettingsBlobAsset>(Allocator.Persistent);

            dstManager.AddComponentData(entity, new Settings()
            {
                SettingsBlobRef = blobAssetReference
            });
        }
    }
}