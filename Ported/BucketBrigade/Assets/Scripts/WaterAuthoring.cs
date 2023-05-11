using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class WaterAuthoring : MonoBehaviour
{
    class Baker : Baker<WaterAuthoring>
    {
        public override void Bake(WaterAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
            var authTransformLocalScale = authoring.transform.localScale;
            AddComponent(entity, new Water
            {
                TargetScale = new float2(authTransformLocalScale.x, authTransformLocalScale.z),
                Volume = 0.8f,
            });
            AddComponent(entity, new PostTransformMatrix());
        }
    }
}

public struct Water : IComponentData
{
    public float2 TargetScale;
    public float Volume;
}