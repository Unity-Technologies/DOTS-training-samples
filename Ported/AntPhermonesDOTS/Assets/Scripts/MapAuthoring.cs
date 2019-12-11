using Unity.Entities;
using UnityEngine;

public class MapAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int mapSize = 128;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Map
        {
            Size = mapSize
        });
    }
}
