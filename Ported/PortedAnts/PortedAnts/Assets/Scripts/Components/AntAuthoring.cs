using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Components
{
    public class AntAuthoring : MonoBehaviour
    {

    }

    class AntBaker : Baker<AntAuthoring>
    {
        public override void Bake(AntAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Ant
            {
                position = float2.zero,
                speed = 1f,
                direction = float2.zero,
                hasFood = false,
                hasSpottedTarget = false
            });
        }
    }

    public struct Ant : IComponentData
    {
        public float2 position;
        public float speed;
        public float2 direction;
        public bool hasFood;
        public bool hasSpottedTarget;
    }
}
