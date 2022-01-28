using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class BeePropertiesAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float ChaseForce = 3f;
    public float Damping = 0.1f;
    public float FlightJitter = 2f;
    public float RotationStiffness = 1f;
    public float TeamAttraction = -1f;
    public float TargetReach = 0.1f;
    public float AttackDashReach = 3f;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BeeProperties
        {
            ChaseForce = ChaseForce,
            Damping = Damping,
            FlightJitter = FlightJitter,
            RotationStiffness = RotationStiffness,
            TeamAttraction = TeamAttraction,
            TargetReach = TargetReach,
            AttackDashReach =  AttackDashReach
        });
    }
}
