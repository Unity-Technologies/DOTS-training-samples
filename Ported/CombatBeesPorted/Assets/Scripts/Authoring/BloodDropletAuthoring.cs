using UnityEngine;
using Unity.Entities;

public class BloodDropletAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Force>(entity);
        dstManager.AddComponent<Velocity>(entity);
    }
}