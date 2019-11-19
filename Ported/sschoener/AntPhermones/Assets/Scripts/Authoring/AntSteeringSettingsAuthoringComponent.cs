using System;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
public class AntSteeringSettingsAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public float TargetSteerStrength;
    public float InwardSteerStrength;
    public float OutwardSteerStrength;
    public float MaxSpeed;
    public float Acceleration;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new AntSteeringSettingsComponent
        {
            TargetSteerStrength = TargetSteerStrength,
            InwardSteerStrength = InwardSteerStrength,
            OutwardSteerStrength = OutwardSteerStrength,
            MaxSpeed = MaxSpeed,
            Acceleration = Acceleration
        });
    }
}
