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

        float3 smPos;
        smPos.x = this.transform.position.x;
        smPos.y = this.transform.position.y;
        smPos.z = this.transform.position.z;
        dstManager.AddComponentData(entity, new SmoothPosition { smPos = smPos + new float3(1, 0, 0) * .01f });
        dstManager.AddComponentData(entity, new SmoothDirection { smDir = new float3(0, 0, 0) });
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

public struct SmoothPosition : IComponentData
{
    public float3 smPos;
}

public struct SmoothDirection : IComponentData
{
    public float3 smDir;
}






