using Unity.Entities;
using UnityEngine;

public class ResourceAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<IsResource>(entity);
        dstManager.AddComponent<HasGravity>(entity);
        dstManager.AddComponent<Velocity>(entity);
    }
}
