using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityGameObject = UnityEngine.GameObject;
using UnityMesh = UnityEngine.Mesh;
using UnityMaterial = UnityEngine.Material;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

namespace Authoring
{
    public class ParticleAuthoring : UnityMonoBehaviour
            , IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new ParticleComponent());
            dstManager.AddComponentData(entity, new Velocity());
            dstManager.AddComponentData(entity, new Lifetime());
            dstManager.AddComponentData(entity, new Scale());
            dstManager.AddComponentData(entity, new Size());
        }
    }
}