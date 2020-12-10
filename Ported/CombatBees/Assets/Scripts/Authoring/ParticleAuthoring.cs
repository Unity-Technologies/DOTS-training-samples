using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public class ParticleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    // authoring fields go here
    public ParticleType.Type particleType;
    public float life;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ParticleType { type = this.particleType });
        dstManager.AddComponentData(entity, new Life { vel = this.life });        
    }
}

// struct BeeTeam : ISharedComponentData
public struct ParticleType : IComponentData
{
    public Type type;
    public enum Type
    {
        Blood = 0,
        SpawnFlash = 1
    };
}

public struct Life : IComponentData
{
    public float vel;
}

public struct LifeDuration : IComponentData
{
    public float vel;
}

public struct Stuck : IComponentData
{
}







