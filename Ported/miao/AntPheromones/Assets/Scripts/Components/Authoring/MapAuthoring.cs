using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class MapAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public AntManager AntManager;
        [Range(0f, 1f)] public float TrailVisibilityModifier = 0.3f;
        [Range(0f, 1f)] public float TrailDecayRate = 0.95f;
        
        public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem _)
        {
            entityManager.AddComponentData(entity, new Map
            {
                Width = this.AntManager.MapWidth,
                ColonyPosition = this.AntManager.ColonyPosition,
                ResourcePosition = this.AntManager.ResourcePosition,

                TrailDecayRate = this.TrailDecayRate,
                TrailVisibilityModifier = this.TrailVisibilityModifier,
                
                Obstacles = Obstacles.Build(this.AntManager)
            });
        }
    }
}