using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class PheromoneRendererAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public AntManager AntManager;
        public MeshRenderer Renderer;
        public Material Material;

        public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem _)
        {
            entityManager.AddSharedComponentData(entity, new PheromoneRenderingSharedComponent
            {
                Material = Material,
                Renderer = Renderer
            });
            entityManager.AddBuffer<PheromoneColourRValueBuffer>(entity)
                         .ResizeUninitialized(this.AntManager.MapWidth * this.AntManager.MapWidth);
        }
    }
}