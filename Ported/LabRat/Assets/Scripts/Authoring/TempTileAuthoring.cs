using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class TempTileAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [System.Flags] enum Wall { None, West = 1, North = 2, East = 4, South = 8 }
    [SerializeField] Wall walls;

    public void Convert(Entity entity, EntityManager mgr, GameObjectConversionSystem conversionSystem)
    {
        mgr.AddComponentData(entity, new Tile {Value = (byte) walls});
        mgr.AddComponentData(entity, new Size { Value = transform.localScale.x });
        mgr.AddComponentData(entity, new PositionXZ {Value = new float2(transform.position.x, transform.position.z)});
    }
}
