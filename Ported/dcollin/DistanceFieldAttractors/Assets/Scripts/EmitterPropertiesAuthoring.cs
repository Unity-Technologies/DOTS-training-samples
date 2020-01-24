using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class EmitterPropertiesAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] public float attraction;
    [SerializeField] public float speedStretch;
    [SerializeField] public float jitter;
    [SerializeField] public Mesh particleMesh;
    [SerializeField] public Material particleMaterial;
    [SerializeField] public Color surfaceColor;
    [SerializeField] public Color interiorColor;
    [SerializeField] public Color exteriorColor;
    [SerializeField] public float exteriorColorDist;
    [SerializeField] public float interiorColorDist;
    [SerializeField] public float colorStiffness;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ParticleColor { value = float4.zero });
        dstManager.AddComponentData(entity, new ParticlePosition { value = new float3(1.0f, 1.0f, 1.0f) });
        dstManager.AddComponentData(entity, new ParticleVelocity { value = float3.zero });

        var singletonEntity = dstManager.CreateEntity(typeof(EmitterPropetiers));
        var singletonGroup = dstManager.CreateEntityQuery(typeof(EmitterPropetiers));
        singletonGroup.SetSingleton<EmitterPropetiers>(new EmitterPropetiers
        {
            Attraction = attraction,
            SpeedStretch = speedStretch,
            Jitter = jitter,
            SurfaceColor = new float4(surfaceColor.r, surfaceColor.g, surfaceColor.b, surfaceColor.a),
            InteriorColor = new float4(interiorColor.r, interiorColor.g, interiorColor.b, interiorColor.a),
            ExteriorColor = new float4(exteriorColor.r, exteriorColor.g, exteriorColor.b, exteriorColor.a),
            ExteriorColorDist = exteriorColorDist,
            InteriorColorDist = interiorColorDist,
            ColorStiffness = colorStiffness,
        });

        dstManager.AddSharedComponentData(entity, new ParticleMesh
        {
            Mesh = particleMesh,
            Material = particleMaterial,
        });
    }
}

