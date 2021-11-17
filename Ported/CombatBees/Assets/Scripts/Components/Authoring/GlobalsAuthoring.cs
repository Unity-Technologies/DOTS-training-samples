using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using UnityTransform = UnityEngine.Transform;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class GlobalsAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject FoodPrefab;
    public UnityGameObject GibletPrefab;
    public int StartingFoodCount;

    //30 10 5

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(FoodPrefab);
        referencedPrefabs.Add(GibletPrefab);
    }

    // This function is required by IConvertGameObjectToEntity
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Globals
        {
            FoodPrefab = conversionSystem.GetPrimaryEntity(FoodPrefab),
            GibletPrefab = conversionSystem.GetPrimaryEntity(GibletPrefab),
            StartingFoodCount = StartingFoodCount
        });

        var mesh = GetComponent<MeshFilter>().mesh;

        Vector3 extents = mesh.bounds.extents;
        extents.Scale(transform.localScale);
        dstManager.AddComponentData(entity, new Bounds
        {
            Value = new AABB { Center = transform.position, Extents = extents }
        });
    }
}