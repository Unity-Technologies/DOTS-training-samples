using UnityEngine;
using Unity.Entities;

public class Rotation_Authoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Directions Direction;
    public float RotationSpeed = 1.0f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new LbRotationSpeed { Value = RotationSpeed } );
        dstManager.AddComponentData(entity, new LbDirection { Value = (byte)Direction });

    }
}
