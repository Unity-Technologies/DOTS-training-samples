using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class AntAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Heading>(entity);
        dstManager.AddComponent<Ant>(entity);
        dstManager.AddComponent<Rotation>(entity);
    }
}
