using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class BeeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<BeeTag>(entity);
    }
}
