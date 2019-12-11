using Unity.Entities;
using UnityEngine;

namespace src
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    [ConverterVersion("christianw", 1)]
    public class TrainAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public LineAuthoring Line;
        public Mesh Mesh;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var carriage = conversionSystem.CreateAdditionalEntity(gameObject);
            dstManager.AddComponent(carriage, typeof(Unity.Transforms.Translation));
            dstManager.AddComponent(carriage, typeof(Unity.Transforms.Rotation));
            dstManager.AddComponent(carriage, typeof(Unity.Transforms.LocalToWorld));
            dstManager.AddComponentData(carriage, new SimpleMeshRenderer()
            {
                Material =  new Material(Shader.Find("Standard")),
                Mesh = Mesh
            });
            dstManager.AddComponentData(carriage, new LinePosition()
            {
                Line = conversionSystem.TryGetPrimaryEntity(Line),
                CurrentIndex = 0,
                Progression = 0,
            });
        }
    }
}