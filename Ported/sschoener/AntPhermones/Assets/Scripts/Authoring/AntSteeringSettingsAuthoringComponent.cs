using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
public class AntSteeringSettingsAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public float RandomSteerStrength;
    public float WallSteerStrength;
    public float PheromoneSteerStrength;
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
            WallSteerStrength = WallSteerStrength,
            PheromoneSteerStrength = PheromoneSteerStrength,
            InwardSteerStrength = InwardSteerStrength,
            OutwardSteerStrength = OutwardSteerStrength,
            RandomSteerStrength = RandomSteerStrength,
            MaxSpeed = MaxSpeed,
            Acceleration = Acceleration
        });
    }
}
