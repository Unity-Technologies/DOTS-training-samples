using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

public class BeeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    // authoring fields go here
    public BeeTeam.TeamColor team;
    public float deathTimer;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddSharedComponentData(entity, new BeeTeam { team = this.team });
        dstManager.AddComponentData(entity, new DeathTimer { dTimer = this.deathTimer });
        dstManager.AddComponentData(entity, new Velocity { vel = float3.zero });
        dstManager.AddComponentData(entity, new Scale { Value = 1.0f });
    }
}

public struct BeeTeam : ISharedComponentData
{
    public TeamColor team;
    public enum TeamColor
    {
        BLUE = 0,
        YELLOW = 1
    };
}

public struct DeathTimer : IComponentData
{
    public float dTimer;
}







