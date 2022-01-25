using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class ArrowAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(entity);
        dstManager.AddComponent<Tile>(entity);
        dstManager.AddComponent<Direction>(entity);
        dstManager.AddComponent<Player>(entity);
        dstManager.AddComponent<PlayTime>(entity);
    }
}
