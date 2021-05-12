
using Unity.Entities;
using UnityEngine;

public class TankAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
{

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Tank { });

        dstManager.AddComponent<BoardPosition>(entity);
        dstManager.AddComponent<CurrentLevel>(entity);
        dstManager.AddComponent<TimeOffset>(entity);
        dstManager.AddComponent<TargetPosition>(entity);
    }
}
