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
                deltaSteering = 0f,
                hasFood = false,
                hasSpottedTarget = false,
                ovx = 0f,
                ovy = 0f,
                vx = 0f,
                vy = 0f
            });
        }
    }

    public struct Ant : IComponentData
    {
        public float2 position;
        public float speed;
        public float facingAngle;
        public float deltaSteering;
        public bool hasFood;
        public bool hasSpottedTarget;
        public float ovx;
        public float ovy;
        public float vx;
        public float vy;
    }
}
