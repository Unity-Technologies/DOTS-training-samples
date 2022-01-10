using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

public class FarmAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int mapSizeX;
    public int mapSizeY;
    
	public Mesh groundMesh;
	public Material groundMaterial;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    { 
        var groundPrefab = conversionSystem.CreateAdditionalEntity(this);
        dstManager.AddComponent<Prefab>(groundPrefab);
        dstManager.AddComponent<Translation>(groundPrefab);
        dstManager.AddComponentData(groundPrefab, new URPMaterialPropertyBaseColor
        {
            Value = new float4(groundMaterial.color.r, groundMaterial.color.g, groundMaterial.color.b, groundMaterial.color.a)
        });
        dstManager.AddComponentData(groundPrefab, new Scale
        {
            Value = 0.1f
        });
        dstManager.AddComponent<Ground>(groundPrefab);

        RenderMeshUtility.AddComponents(
            groundPrefab,
            dstManager,
            new RenderMeshDescription(
                groundMesh,
                groundMaterial));

        var renderMesh = dstManager.GetSharedComponentData<RenderMesh>(groundPrefab);
        renderMesh.castShadows = ShadowCastingMode.On;
        dstManager.SetSharedComponentData(groundPrefab, renderMesh);

        dstManager.AddComponentData(entity, new FarmConfig
        {
            MapSizeX = mapSizeX,
            MapSizeY = mapSizeY,
            GroundPrefab = groundPrefab,
            TilledGroundColor = new float4(groundMaterial.color.r / 2, groundMaterial.color.g / 2, groundMaterial.color.b / 2, groundMaterial.color.a)
        });
    }
}