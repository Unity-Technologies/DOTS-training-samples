using Unity.Entities;
using UnityEngine;

public struct ConvertToCloth : IComponentData
{
    private byte dummy;
}

public class ConvertToClothComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<ConvertToCloth>(entity, new ConvertToCloth { });
    }
}