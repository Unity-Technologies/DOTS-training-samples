using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace AntPheromones_ECS
{
    public class AntSteeringMovementAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float MaxSpeed;
        public float Acceleration;
        
        public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem)
        {
            entityManager.AddComponentData(entity, new SteeringMovement
            {
                MaxSpeed = this.MaxSpeed,
                Acceleration = this.Acceleration
            });
        }
    }
}