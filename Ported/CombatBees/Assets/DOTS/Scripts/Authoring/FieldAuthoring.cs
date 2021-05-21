using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FieldAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Bounds>(entity);
        dstManager.AddComponent<IsArena>(entity);

        var translation = transform.position;
        var extents = math.abs(transform.localScale / 2);

        dstManager.AddComponentData(entity, new Bounds
        {
            Value = new AABB
            {
                Center = translation,
                Extents = extents
            }
        });
    }
}
