using Unity.Entities;
using UnityEngine;

public class MapAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    // TODO: Enforce power of 2 map size.
    public int mapSize = 128;
    public int tileCount = 8;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Map
        {
            Size = mapSize,
            TileSize = mapSize / tileCount
        });
    }
}
