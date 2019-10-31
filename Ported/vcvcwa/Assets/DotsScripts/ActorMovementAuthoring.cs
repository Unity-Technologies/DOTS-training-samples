using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequiresEntityConversion]
[AddComponentMenu("DOTS Samples/Positions/ActorMovement")]
public class ActorMovementAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float speed;
    public float2 goalPosition;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new ActorMovementComponent()
        {
            speed = speed,
            targetPosition = goalPosition,
            position = new float2(transform.position.x, transform.position.y)
        };
        
        dstManager.AddComponentData(entity, data);
    }
}
