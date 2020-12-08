using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

public class ResourceAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float deathTimer;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Velocity { vel = float3.zero });
        dstManager.AddComponentData(entity, new Scale { Value = 1.0f });
        dstManager.AddComponentData(entity, new StackIndex { index = 0 });
        dstManager.AddComponentData(entity, new GridX { gridX = 0 });
        dstManager.AddComponentData(entity, new GridY { gridY = 0 });
        dstManager.AddComponentData(entity, new DeathTimer { dTimer = this.deathTimer });
    }
}

public struct StackIndex : IComponentData
{
    public int index;
}

public struct GridX : IComponentData
{
    public int gridX;
}

public struct GridY : IComponentData
{
    public int gridY;
}