using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CatAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Creature>(entity);
        dstManager.AddComponent<Cat>(entity);
        dstManager.AddComponent<Tile>(entity);
        dstManager.AddComponent<Direction>(entity);
        dstManager.AddComponent<TileLerp>(entity);
        dstManager.AddComponent<Scale>(entity);
    }
}
