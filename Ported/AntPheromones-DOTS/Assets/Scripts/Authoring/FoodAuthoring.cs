using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class FoodAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Goal>(entity);
    }
}
