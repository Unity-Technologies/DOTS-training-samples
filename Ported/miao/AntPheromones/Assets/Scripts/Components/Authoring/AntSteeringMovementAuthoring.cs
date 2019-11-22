using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace AntPheromones_ECS
{
    public class AntSteeringMovementAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float MaxSpeed;
        [Range(0f, 1f)] public float Acceleration;
        
        public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem _)
        {
            entityManager.AddComponentData(entity, new SteeringMovementComponent
            {
                MaxSpeed = this.MaxSpeed,
                Acceleration = this.Acceleration
            });
        }
    }
}