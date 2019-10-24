using Unity.Entities;
using UnityEngine;

public struct OverlayComponentTag : IComponentData
{}

public class AuthoringOverlayComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<OverlayComponentTag>(entity);
    }
}
