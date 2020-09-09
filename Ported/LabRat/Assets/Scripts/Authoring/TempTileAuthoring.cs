using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

class TempTileAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [FormerlySerializedAs("walls")] [SerializeField] Tile.Attributes attributes;

    public void Convert(Entity entity, EntityManager mgr, GameObjectConversionSystem conversionSystem)
    {
        mgr.AddComponentData(entity, new Tile { Value = attributes});
        mgr.AddComponentData(entity, new Size { Value = transform.localScale.x });
        mgr.AddComponentData(entity, new PositionXZ {Value = new float2(transform.position.x, transform.position.z)});
    }
}
