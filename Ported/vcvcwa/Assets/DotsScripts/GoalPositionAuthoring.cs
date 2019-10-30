using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
[AddComponentMenu("DOTS Samples/Positions/GoalPosition")]
public class GoalPositionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Vector2 goalPosition;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new GoalPositionComponent()
        {
            position = goalPosition
        };
        
        dstManager.AddComponentData(entity, data);
    }
}
