
using System.Collections.Generic;

using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class TankAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Tank { });

        dstManager.AddComponent<Rotation>(entity);
        dstManager.AddComponent<BoardPosition>(entity);
        dstManager.AddComponent<TimeOffset>(entity);
        dstManager.AddComponent<TargetPosition>(entity);
    }
}
