using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AntSimulationTransform2DAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var transformPosition = transform.position;
        dstManager.AddComponentData(entity, new AntSimulationTransform2D()
        {
            position = new float2(transformPosition.x, transformPosition.y),
        });
        
        // NW: Better ECS chunk cache cohesion without all this:
        dstManager.RemoveComponent<Translation>(entity);
        dstManager.RemoveComponent<Rotation>(entity);
        dstManager.RemoveComponent<Scale>(entity);
        //dstManager.RemoveComponent<LocalToWorld>(entity);
        dstManager.RemoveComponent<LinkedEntityGroup>(entity);
    }
}
