using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityGameObject = UnityEngine.GameObject;
using UnityMesh = UnityEngine.Mesh;
using UnityMaterial = UnityEngine.Material;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

namespace Authoring
{
    public class BeeAuthoring : UnityMonoBehaviour
            , IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TargetEntity());
            dstManager.AddComponentData(entity, new TargetType());
            dstManager.AddComponentData(entity, new AttractionRepulsion());
            dstManager.AddComponentData(entity, new NonUniformScale());
            dstManager.AddComponentData(entity, new BeeMovement());
            dstManager.AddComponentData(entity, new MovementSmoothing());
            dstManager.AddComponentData(entity, new Velocity());
        }
    }
}