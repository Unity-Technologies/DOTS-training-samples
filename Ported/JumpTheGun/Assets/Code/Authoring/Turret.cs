
using System.Collections.Generic;

using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class TurretAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Turret { });

        dstManager.AddComponent<Translation>(entity);
        dstManager.AddComponent<Rotation>(entity);
    }
}
