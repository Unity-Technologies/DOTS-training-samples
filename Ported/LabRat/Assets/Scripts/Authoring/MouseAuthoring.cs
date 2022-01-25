using Unity.Entities;
using UnityEngine;

public class MouseAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Mouse>(entity);
        dstManager.AddComponent<Tile>(entity);
        dstManager.AddComponent<Direction>(entity);
        dstManager.AddComponent<TileLerp>(entity);
    }
}