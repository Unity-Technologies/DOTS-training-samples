using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct MarkerAuthoring : IComponentData
{

}
/*
public class MarkerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject markerPrefab;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var markerEntity = new Marker
        {
            markerPrefab = conversionSystem.GetPrimaryEntity(this.markerPrefab)
        };

        dstManager.AddComponentData(entity, markerEntity);

    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(this.markerPrefab);
    }    
}

public struct Marker : IComponentData
{
    public Entity markerPrefab;
}
*/
