using Unity.Entities;
using Unity.Mathematics;

public struct ParticleEmitterSettings : IComponentData
{
    public float Attraction;
    public float SpeedStretch;
    public float Jitter;
    public float4 SurfaceColor;
    public float4 InteriorColor;
    public float4 ExteriorColor;
    public float ExteriorColorDistance;
    public float InteriorColorDistance;
    public float ColorStiffness;
    public Random rng;
}

public class ParticleEmitterSettingsAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public float Attraction;
    public float SpeedStretch;
    public float Jitter;
    public UnityEngine.Color SurfaceColor;
    public UnityEngine.Color InteriorColor;
    public UnityEngine.Color ExteriorColor;
    public float ExteriorColorDistance;
    public float InteriorColorDistance;
    public float ColorStiffness;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var coolRng = new Random();
        coolRng.InitState();
        dstManager.AddComponentData(entity, new ParticleEmitterSettings 
        {
            Attraction = Attraction,
            SpeedStretch = SpeedStretch,
            Jitter = Jitter,
            SurfaceColor = new float4(SurfaceColor.r, SurfaceColor.g, SurfaceColor.b, SurfaceColor.a),
            InteriorColor = new float4(InteriorColor.r, InteriorColor.g, InteriorColor.b, InteriorColor.a),
            ExteriorColor = new float4(ExteriorColor.r, ExteriorColor.g, ExteriorColor.b, ExteriorColor.a),
            ExteriorColorDistance = ExteriorColorDistance,
            InteriorColorDistance = InteriorColorDistance,
            ColorStiffness = ColorStiffness,
            rng = coolRng
        });
    }
}
