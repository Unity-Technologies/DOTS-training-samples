using Unity.Entities;
using UnityEngine;

namespace src
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    [ConverterVersion("martinsch", 4)]
    public class TrainAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public LineAuthoring Line;
        public Mesh Mesh;
        public float TargetSpeed = 100;
        public int NumberOfCarriages = 3;
        public float CarriageDistance = 2.0f;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var previousEntity = Entity.Null;
            
            for (int i = 0; i < NumberOfCarriages; i++)
            {
                var carriage = conversionSystem.CreateAdditionalEntity(gameObject);
                dstManager.AddComponent(carriage, typeof(Unity.Transforms.Translation));
                dstManager.AddComponent(carriage, typeof(Unity.Transforms.Rotation));
                dstManager.AddComponent(carriage, typeof(Unity.Transforms.LocalToWorld));
                dstManager.AddComponent(carriage, typeof(NeedsVisuals));

                var material = new Material(Shader.Find("Standard"));
                if (i == 0)
                {
                    material.color = Color.blue;
                }
                
                dstManager.AddComponentData(carriage, new SimpleMeshRenderer()
                {
                    Material = material,
                    Mesh = Mesh
                });
                dstManager.AddComponentData(carriage, new LinePosition()
                {
                    Line = conversionSystem.TryGetPrimaryEntity(Line),
                    CurrentIndex = 0,
                    Progression = 0,
                });
                dstManager.AddComponentData(carriage, new Speed()
                {
                    Value = 0,
                });
                
                if (i == 0)
                {
                    dstManager.AddComponentData(carriage, new TargetSpeed()
                    {
                        Value = TargetSpeed,
                    });
                }
                else
                {
                    dstManager.AddComponentData(carriage, new FollowTranslation()
                    {
                        Target = previousEntity,
                        Distance = CarriageDistance
                    });
                }

                previousEntity = carriage;
            }
        }
    }
}