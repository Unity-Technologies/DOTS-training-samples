using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityMeshRenderer = UnityEngine.MeshRenderer;

namespace Authoring
{
    public class BarAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
    {
        public UnityEngine.Color color;

        public void Convert(Entity entity, EntityManager dstManager
            , GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Color
            {
                color = new float4(this.color.r, this.color.g, this.color.b, this.color.a)
            });
            dstManager.AddComponentData(entity, new Components.Bar
            {
                thickness = 0.1f,
                oldDirection = new float3(0.0f, 1.0f, 0.0f)
            });
        }
    }
}
