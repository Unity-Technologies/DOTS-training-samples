using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class TempTileAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    
    [SerializeField] Tile.Attributes walls;

    public void Convert(Entity entity, EntityManager mgr, GameObjectConversionSystem conversionSystem)
    {
        mgr.AddComponentData(entity, new Tile { Value = walls});
        mgr.AddComponentData(entity, new Size { Value = transform.localScale.x });
        mgr.AddComponentData(entity, new PositionXZ {Value = new float2(transform.position.x, transform.position.z)});
    }
}
