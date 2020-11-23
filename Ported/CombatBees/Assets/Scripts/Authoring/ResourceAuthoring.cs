using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

public class ResourceAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    // authoring fields go here
    public int team;
    public float deathTimer;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BeeTeam { team = this.team });
        dstManager.AddComponentData(entity, new DeathTimer { dTimer = this.deathTimer });
        dstManager.AddComponentData(entity, new Velocity { vel = float3.zero });
        dstManager.AddComponentData(entity, new Scale { Value = 1.0f });
    }
}

public struct StackIndex : IComponentData
{
    public float index;
}

public struct GridX : IComponentData
{
    public float gridX;
}

public struct GridY : IComponentData
{
    public float gridY;
}