using Unity.Entities;
using UnityEngine;

public struct OverlayComponentTag : IComponentData
{}

public struct OverlayPlacementTickComponent : IComponentData
{
    public float Tick;
}

public class AuthoringOverlayComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<OverlayComponentTag>(entity);
        dstManager.AddComponent<OverlayPlacementTickComponent>(entity);
    }
}
