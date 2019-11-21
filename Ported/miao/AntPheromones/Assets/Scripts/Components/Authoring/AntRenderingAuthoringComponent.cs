using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class AntRenderSettingsAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Color CarryColour;
        public Color SearchColour;
        public Vector3 Scale;
        public Material Material;
        public Mesh Mesh;

        public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem)
        {
            entityManager.AddComponentData(entity, new AntRenderingComponent
            {
                CarryColour = this.CarryColour,
                SearchColour = this.SearchColour,
                Scale = this.Scale
            });
            entityManager.AddSharedComponentData(entity, new RenderData
            {
                Material = Material,
                Mesh = Mesh
            });
        }
    }
}
