
using System.Collections.Generic;

using Unity.Entities;
using UnityEngine;

public class BoardAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
    }
}
