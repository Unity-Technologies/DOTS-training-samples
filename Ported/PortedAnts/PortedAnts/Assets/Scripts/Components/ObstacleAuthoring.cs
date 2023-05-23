using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Components
{
    public class ObstacleAuthoring : MonoBehaviour
    {
    }
    
    class ObstacleBaker : Baker<ObstacleAuthoring>
    {
        public override void Bake(ObstacleAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Obstacle
            {
                radius = 1f,
                position = new float2(0,0)
            });
        }
    }

    public struct Obstacle : IComponentData
    {
        public float radius;
        public float2 position;
    }
}