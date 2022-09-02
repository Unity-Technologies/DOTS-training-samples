using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
public class ParticleSparkAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float Lifetime = 1f;
    public float Drag = 0.01f;
    public float StretchFactor = 5f;
    public float StretchMax = 3f;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ParticleSize { Size = transform.localScale });
        dstManager.AddComponentData(entity, new NonUniformScale { Value = 1f});
        dstManager.AddComponentData(entity, new ParticleVelocity());
        dstManager.AddComponentData(entity, new ParticleOrientTowardsVelocity());
        dstManager.AddComponentData(entity, new ParticleLifetime { MaxLifetime = Lifetime });
        dstManager.AddComponentData(entity, new ParticleDrag { Drag = Drag });
        dstManager.AddComponentData(entity, new ParticleStretchWithVelocity { Factor = StretchFactor, Max = StretchMax });
        dstManager.AddComponentData(entity, new ParticleShrinkOverLifetime {  });
    }
}
