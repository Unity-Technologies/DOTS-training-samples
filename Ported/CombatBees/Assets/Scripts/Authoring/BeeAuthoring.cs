using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public class BeeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    // authoring fields go here
    public BeeTeam.TeamColor team;
    public float deathTimer;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BeeTeam { team = this.team });
        dstManager.AddComponentData(entity, new DeathTimer { dTimer = this.deathTimer });
        dstManager.AddComponentData(entity, new Velocity { vel = float3.zero });
        dstManager.AddComponentData(entity, new NonUniformScale { Value = new float3(1.0f, 1.0f, 1.0f) });
        dstManager.AddComponentData(entity, new Size { value = 1.0f });

        float3 smPos;
        smPos.x = this.transform.position.x;
        smPos.y = this.transform.position.y;
        smPos.z = this.transform.position.z;
        dstManager.AddComponentData(entity, new SmoothPosition { smPos = smPos + new float3(1, 0, 0) * .01f });
        dstManager.AddComponentData(entity, new SmoothDirection { smDir = new float3(0, 0, 0) });

        //var baseColor = conversionSystem.GetPrimaryEntity(gameObject);
        //dstManager.AddComponent<URPMaterialPropertyBaseColor>(baseColor);
    }
}

// struct BeeTeam : ISharedComponentData
public struct BeeTeam : IComponentData
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

public struct Size : IComponentData
{
    public float value;
}






