using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
public class AntRenderSettingsAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public Color CarrierColor;
    public Color SearcherColor;
    public Vector3 Scale;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new AntRenderSettingsComponent
        {
            CarrierColor = CarrierColor,
            SearcherColor = SearcherColor,
            Scale = Scale
        });
    }
}