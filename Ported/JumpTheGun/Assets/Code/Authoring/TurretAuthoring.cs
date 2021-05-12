
using Unity.Entities;
using UnityEngine;

public class TurretAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
{

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new CurrentLevel());
        dstManager.AddComponentData(entity, new Turret());
    }
}
