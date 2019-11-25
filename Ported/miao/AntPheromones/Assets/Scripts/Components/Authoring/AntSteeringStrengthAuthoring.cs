using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class AntSteeringStrengthAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float Random;
        public float Goal;
        
        public float Pheromone;
        public float Wall;
       
        public float Inward;
        public float Outward;
        
        public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem _)
        {
            entityManager.AddComponentData(entity, new SteeringStrength
            {
                Goal = this.Goal,
                Random = this.Random,
                Wall = this.Wall,
                Pheromone = this.Pheromone,
                Inward = this.Inward,
                Outward = this.Outward
            });
        }
    }
}