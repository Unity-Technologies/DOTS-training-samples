using Unity.Entities;
using UnityEngine;

public class BloodAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<IsOriented>(entity);
        dstManager.AddComponent<IsStretched>(entity);
    }
}
