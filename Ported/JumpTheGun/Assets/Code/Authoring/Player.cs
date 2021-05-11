
using System.Collections.Generic;

using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Player { });

        dstManager.AddComponent<Arc>(entity);
        dstManager.AddComponent<Translation>(entity);
        dstManager.AddComponent<Direction>(entity);
        dstManager.AddComponent<TargetPosition>(entity);
    }
}
