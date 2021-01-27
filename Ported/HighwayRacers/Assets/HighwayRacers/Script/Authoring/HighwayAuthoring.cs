using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HighwayAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public GameObject StraightPiecePrefab;
    public GameObject CurvePiecePrefab;
    
    
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(StraightPiecePrefab);
        referencedPrefabs.Add(CurvePiecePrefab);
    }
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new HighwayPrefabs()
        {
            StraightPiecePrefab = conversionSystem.GetPrimaryEntity(StraightPiecePrefab),
            CurvePiecePrefab = conversionSystem.GetPrimaryEntity(CurvePiecePrefab),
        });

    
    }
}
