using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CarPropertiesAuthoring : MonoBehaviour
{
    public float DefaultSpeed;
    public float OvertakeSpeed;
    public float DistanceToCarBeforeOvertaking;
    public float OvertakeEagerness;
    public float MergeSpace;
    public float Acceleration;
}

[UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
public class CarConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((CarPropertiesAuthoring car) =>
        {
            var entity = GetPrimaryEntity(car);

            DstEntityManager.AddComponentData(entity,
                new CarProperties
                {
                    DefaultSpeed = car.DefaultSpeed,
                    OvertakeSpeed = car.OvertakeSpeed,
                    DistanceToCarBeforeOvertaking = car.DistanceToCarBeforeOvertaking,
                    OvertakeEagerness = car.OvertakeEagerness,
                    MergeSpace = car.MergeSpace,
                    Acceleration = car.Acceleration
                });

            DstEntityManager.RemoveComponent<Translation>(entity);
            DstEntityManager.RemoveComponent<Rotation>(entity);
            DstEntityManager.RemoveComponent<Scale>(entity);
            DstEntityManager.RemoveComponent<NonUniformScale>(entity);
        });
    }
}