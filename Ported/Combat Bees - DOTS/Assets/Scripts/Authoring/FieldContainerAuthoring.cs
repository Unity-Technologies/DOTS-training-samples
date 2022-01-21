using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class FieldContainerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Transform FieldContainer;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Container
        {
            Center = FieldContainer.position,
            // Used for resource spawn on click
            Dimensions = FieldContainer.localScale,
            // Used for calculating collisions etc.
            MinPosition = new float3(
                FieldContainer.position.x - FieldContainer.localScale.x/2,
                FieldContainer.position.y - FieldContainer.localScale.y/2,
                FieldContainer.position.z - FieldContainer.localScale.z/2),
            MaxPosition = new float3(
                FieldContainer.position.x + FieldContainer.localScale.x/2,
                FieldContainer.position.y + FieldContainer.localScale.y/2,
                FieldContainer.position.z + FieldContainer.localScale.z/2)
        });
    }
}
