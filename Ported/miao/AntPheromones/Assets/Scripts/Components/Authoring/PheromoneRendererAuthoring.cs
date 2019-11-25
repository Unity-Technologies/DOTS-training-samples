using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class PheromoneRendererAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public AntManager AntManager;
        public MeshRenderer Renderer;
        public Material Material;

        public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem _)
        {
            entityManager.AddSharedComponentData(entity, new PheromoneSharedRendering
            {
                Material = Material,
                Renderer = Renderer
            });
            var buffer = entityManager.AddBuffer<PheromoneColourRValueBuffer>(entity);
            buffer.ResizeUninitialized(this.AntManager.MapWidth * this.AntManager.MapWidth);

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0;
            }
        }
    }
}