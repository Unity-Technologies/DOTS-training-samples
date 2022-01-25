using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class ExitAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(entity);
        dstManager.AddComponent<PropagateColor>(entity);
        dstManager.AddComponent<Tile>(entity);
        dstManager.AddComponent<Exit>(entity);
        dstManager.AddComponent<PlayerOwned>(entity);
    }
}