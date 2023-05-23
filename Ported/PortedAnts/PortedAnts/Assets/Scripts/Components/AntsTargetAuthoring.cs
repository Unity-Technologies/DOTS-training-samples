using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Components
{
    public class AntsTargetAuthoring : MonoBehaviour
    {

    }

    class AntsTargetBaker : Baker<AntsTargetAuthoring>
    {
        public override void Bake(AntsTargetAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AntsTarget
            {
                position = new float2(0, 0),
                radius = 1f,
                isHome = true
            });
        }
    }

    public struct AntsTarget : IComponentData
    {
        public float radius;
        public float2 position;
        public bool isHome;
    }
}