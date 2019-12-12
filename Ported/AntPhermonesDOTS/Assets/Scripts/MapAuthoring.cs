using Unity.Entities;
using UnityEngine;

public class MapAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    // TODO: Enforce power of 2 map size.
    public int mapSize = 128;
    public int tileCount = 32;
    
    [Range(0f,1f)]
    public float trailDecay = 0.1f;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Map
        {
            Size = mapSize,
            TileSize = mapSize / tileCount,
            TrailDecay = trailDecay
        });
    }
}
