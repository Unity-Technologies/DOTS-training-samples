using Unity.Entities;
using Unity.Collections;
using Unity.Rendering;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

public class BeeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
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

public struct BeeTeam : IComponentData
{
    public int team;
}

public struct DeathTimer : IComponentData
{
    public float dTimer;
}







