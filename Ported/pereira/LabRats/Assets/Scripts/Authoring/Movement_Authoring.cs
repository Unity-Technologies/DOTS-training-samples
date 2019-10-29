using UnityEngine;
using Unity.Entities;

/// <summary>
/// Class made for testing purposes. According the struct direction is going to be
/// added certain kind of Component
/// </summary>
public class Movement_Authoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Directions Direction;
    public float Speed = 1.0f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new LbMovementSpeed { Value = Speed });
        dstManager.AddComponentData(entity, new LbDistanceToTarget() { Value = 1 });
        dstManager.AddComponentData(entity, new LbDirection() { Value = (byte)Direction });
        dstManager.AddComponentData(entity, new LbMovementTarget());
    }
}
