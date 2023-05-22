using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Components
{
    public class ObstacleAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
    }
    
    class Baker : Baker<ObstacleAuthoring>
    {
        public override void Bake(ObstacleAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new Obstacle
            {
                prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Renderable),
                radius = 1f,
                position = new float2(0,0)
            });
        }
    }

    public struct Obstacle : IComponentData
    {
        public float radius;
        public Entity prefab;
        public float2 position;
    }
}