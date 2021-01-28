using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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
        var straightEntity = conversionSystem.GetPrimaryEntity(StraightPiecePrefab);
        var curvedEntity = conversionSystem.GetPrimaryEntity(CurvePiecePrefab);
        
        dstManager.AddComponentData(entity, new HighwayPrefabs()
        {
            StraightPiecePrefab = straightEntity,
            CurvePiecePrefab = curvedEntity,
        });

        dstManager.AddComponent<NonUniformScale>(straightEntity);
    }
}
