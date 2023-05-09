using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Metro
{
    public class PassengerAuthoring : MonoBehaviour
    {
        class Baker : Baker<PassengerAuthoring>
        {
            public override void Bake(PassengerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
                AddComponent(entity, new PassengerComponent
                {
                    Color = new float3(1, 0, 1),
                    Height = 1f
                });
            }
        }
    }

    public struct PassengerComponent : IComponentData
    {
        public float3 Color;
        public float Height;
    }
}
