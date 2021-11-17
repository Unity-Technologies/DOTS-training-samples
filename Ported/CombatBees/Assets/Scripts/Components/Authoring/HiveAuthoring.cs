using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class HiveAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject BeePrefab;
    public int Team;

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BeePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        if (Team == 0)
        {
            dstManager.AddComponentData(entity, new HiveTeam0
            {
                BeePrefab = conversionSystem.GetPrimaryEntity(BeePrefab)
            });
            dstManager.AddComponentData(entity, new TeamID { Value = 0 });
        }
        else if (Team == 1)
        {
            dstManager.AddComponentData(entity, new HiveTeam1
            {
                BeePrefab = conversionSystem.GetPrimaryEntity(BeePrefab)
            });
            dstManager.AddComponentData(entity, new TeamID { Value = 1 });
        }

        var mesh = GetComponent<MeshFilter>().mesh;
        Vector3 extents = mesh.bounds.extents;
        extents.Scale(transform.localScale);
        dstManager.AddComponentData(entity, new Bounds
        {
            Value = new AABB { Center = transform.position, Extents = extents }
        });
    }
}