using System;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class RoadGeneratorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Mesh CarMesh;
    public Material CarMaterial;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<CarSetupTagComponent>(entity);
        dstManager.AddSharedComponentData(entity, new RenderMesh()
        { 
            mesh = CarMesh,
            material = CarMaterial
        });
    }
}
