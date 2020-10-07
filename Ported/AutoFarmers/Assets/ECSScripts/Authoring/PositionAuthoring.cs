using Unity.Entities;
using Unity.Mathematics;
//using UnityEngine;

[GenerateAuthoringComponent]
public struct Position : IComponentData
{
    public float2 Value;
}

// public class PositionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
// {
//     public UnityEngine.Vector2 Position;
//
//     public void Convert(Entity entity, EntityManager dstManager,
//         GameObjectConversionSystem conversionSystem)
//     {
//         dstManager.AddComponentData(entity, new Position
//         {
//             Value = new float2(Position.x, Position.y)
//         });
//     }
// }
