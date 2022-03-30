using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

namespace Authoring
{
    public class BarAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
    {
        public UnityEngine.Color color;

        public void Convert(Entity entity, EntityManager dstManager
            , GameObjectConversionSystem conversionSystem)
        {
            var colorVector = new float4(color.r, color.g, color.b, color.a);
            dstManager.AddComponentData(entity, new Bar());
            dstManager.AddComponentData(entity, new NonUniformScale());
            dstManager.AddComponentData(entity, new URPMaterialPropertyBaseColor {Value = colorVector});
        }
    }
}
