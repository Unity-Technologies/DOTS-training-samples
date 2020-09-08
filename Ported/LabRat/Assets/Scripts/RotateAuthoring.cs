using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Rotate : IComponentData
{
    public float RotationSpeed;
}

public class RotateAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float RotationSpeedDegrees;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Rotate
        {
            RotationSpeed = math.radians(RotationSpeedDegrees)
        });
    }
}