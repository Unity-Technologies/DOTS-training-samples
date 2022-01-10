using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

public class FarmAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Mesh particleMesh;
    public Material particleMaterial;
    public float spinRate;
    public float upwardSpeed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var prefab = conversionSystem.CreateAdditionalEntity(this);
        dstManager.AddComponent<Prefab>(prefab);
        dstManager.AddComponent<Particle>(prefab);
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(prefab);
        dstManager.AddComponent<Translation>(prefab);
        dstManager.AddComponent<Scale>(prefab);

        RenderMeshUtility.AddComponents(
            prefab,
            dstManager,
            new RenderMeshDescription(
                particleMesh,
                particleMaterial));

        var renderMesh = dstManager.GetSharedComponentData<RenderMesh>(prefab);
        renderMesh.castShadows = ShadowCastingMode.On;
        dstManager.SetSharedComponentData(prefab, renderMesh);

        dstManager.AddComponentData(entity, new ParticleManagerConfig
        {
            ParticlePrefab = prefab,
            SpinRate = spinRate,
            UpwardSpeed = upwardSpeed
        });
    }
}