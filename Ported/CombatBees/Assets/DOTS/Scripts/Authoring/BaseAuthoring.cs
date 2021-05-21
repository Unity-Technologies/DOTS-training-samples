using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BaseAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int TeamId;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Bounds>(entity);
        dstManager.AddComponent<Team>(entity);

        dstManager.AddComponentData(entity, new Team
        {
            Id = TeamId
        });

        if (TeamId == 0)
        {
            dstManager.AddComponent<YellowBase>(entity);
        }
        else
        {
            dstManager.AddComponent<BlueBase>(entity);
        }

        var translation = transform.position;
        var extents = transform.localScale / 2;
        extents = math.abs(extents);

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