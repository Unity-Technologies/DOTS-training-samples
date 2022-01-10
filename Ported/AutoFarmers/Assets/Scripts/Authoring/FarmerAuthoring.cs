using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

public class FarmerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Mesh farmerMesh;
    public Material farmerMaterial;
	public int moneyForFarmers;
	public int initialFarmerCount;
	public int maxFarmerCount;
	[Range(0f,1f)]
	public float moveSmooth;

    public int farmerRange;
    public float walkSpeed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var prefab = conversionSystem.CreateAdditionalEntity(this);
        dstManager.AddComponent<Prefab>(prefab);
        dstManager.AddComponent<Translation>(prefab);
        dstManager.AddComponentData(prefab, new URPMaterialPropertyBaseColor
        {
            Value = new float4(farmerMaterial.color.r, farmerMaterial.color.g, farmerMaterial.color.b, farmerMaterial.color.a)
        });
        dstManager.AddComponentData(prefab, new Scale
        {
            Value = 1f
        });
        dstManager.AddComponent<Farmer>(prefab);
        dstManager.AddComponent<Path>(prefab);

        RenderMeshUtility.AddComponents(
            prefab,
            dstManager,
            new RenderMeshDescription(
                farmerMesh,
                farmerMaterial));

        var renderMesh = dstManager.GetSharedComponentData<RenderMesh>(prefab);
        renderMesh.castShadows = ShadowCastingMode.On;
        dstManager.SetSharedComponentData(prefab, renderMesh);

        dstManager.AddComponentData(entity, new FarmerConfig
        {
            FarmerPrefab = prefab,
            MoneyForFarmers = moneyForFarmers,
            InitialFarmerCount = initialFarmerCount,
            MaxFarmerCount = maxFarmerCount,
            FarmerRange = farmerRange,
            WalkSpeed = walkSpeed,
            MoveSmooth = moveSmooth
        });
    }
}