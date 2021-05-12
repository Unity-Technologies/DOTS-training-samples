
using System.Collections.Generic;

using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class PlatformAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Platform { });

        //dstManager.AddComponent<Translation>(entity);
        //dstManager.AddComponent<BallTrajectory>(entity);

        dstManager.AddComponent<BoardPosition>(entity);
        dstManager.AddComponent<CurrentLevel>(entity);
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(entity);
    }
}
