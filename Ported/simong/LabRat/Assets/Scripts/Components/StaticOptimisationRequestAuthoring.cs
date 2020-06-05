using Unity.Entities;
using UnityEngine;

public class StaticOptimisationRequestAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<StaticOptimisationRequest>(entity);
    }
}