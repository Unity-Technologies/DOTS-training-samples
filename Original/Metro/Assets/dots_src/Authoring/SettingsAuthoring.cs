using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SettingsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float MaxSpeed;
    public float CarriageSizeWithMargins;
    public float TimeAtStation;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<Settings>(entity, new Settings
        {
            MaxSpeed = MaxSpeed,
            CarriageSizeWithMargins = CarriageSizeWithMargins,
            TimeAtStation = TimeAtStation
        });
    }
}
