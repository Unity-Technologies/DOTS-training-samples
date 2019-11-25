using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class AntRenderingAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Color CarryColour;
        public Color SearchColour;
        public float3 Scale;
        public Material Material;
        public Mesh Mesh;
        
        public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem _)
        {
            entityManager.AddComponentData(entity, new AntIndividualRendering
            {
                CarryColour = this.CarryColour,
                SearchColour = this.SearchColour,
                Scale = this.Scale
            });
            entityManager.AddSharedComponentData(entity, new AntSharedRendering
            {
                Material = this.Material,
                Mesh = this.Mesh
            });
        }
    }
}