using Components;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PassengerAuthoring : MonoBehaviour
{
    public float MinHeight;
    public float MaxHeight;
    class Baker : Baker<PassengerAuthoring>
    {
        public override void Bake(PassengerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PassengerComponent
            {
                Color = new float3(1, 0, 1),
                Height = authoring.MaxHeight
            });
            AddComponent(entity, new PassengerOnboarded());
            SetComponentEnabled<PassengerOnboarded>(entity, false);
        }
    }
}

public struct PassengerComponent : IComponentData
{
    public float3 Color;
    public float Height;
}

