using Unity.Entities;
using UnityEngine;

public class LaneAssignmentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddSharedComponentData(entity, new LaneAssignment() {Value = 0});
    }
} 