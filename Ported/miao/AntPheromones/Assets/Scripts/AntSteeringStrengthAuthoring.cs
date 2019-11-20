using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class AntSteeringStrengthAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float Random;
        public float Target;
        
        public float Pheromone;
        public float Wall;
       
        public float Inward;
        public float Outward;
        
        public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem)
        {
            entityManager.AddComponentData(entity, new SteeringStrength
            {
                Target = this.Target,
                Random = this.Random,
                Wall = this.Wall,
                Pheromone = this.Pheromone,
                Inward = this.Inward,
                Outward = this.Outward
            });
        }
    }
}