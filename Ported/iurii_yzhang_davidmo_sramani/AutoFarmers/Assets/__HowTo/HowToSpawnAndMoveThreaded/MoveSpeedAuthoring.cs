using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class MoveSpeedAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Vector3 velocity;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MoveSpeed { Value = velocity });
    }
}
