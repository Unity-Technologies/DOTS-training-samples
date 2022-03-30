using Components;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

namespace Authoring
{
    public class ParticleAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager
            , GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Particle());
            dstManager.AddComponentData(entity, new URPMaterialPropertyBaseColor());
            dstManager.AddComponentData(entity, new Scale());
        }
    }
}