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
                speed = 10f,
                facingAngle = 1f,
                hasFood = false,
                hasSpottedTarget = false
            });
        }
    }

    public struct Ant : IComponentData
    {
        public float2 position;
        public float speed;
        public float facingAngle;
        public bool hasFood;
        public bool hasSpottedTarget;
    }
}
