using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

[RequiresEntityConversion]
public class AntRenderSettingsAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public Color CarrierColor;
    public Color SearcherColor;
    public Vector3 Scale;
    public Material Material;
    public Mesh Mesh;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new AntRenderSettingsComponent
        {
            CarrierColor = CarrierColor,
            SearcherColor = SearcherColor,
            Scale = Scale
        });
        dstManager.AddSharedComponentData(entity, new RenderData
        {
            Material = Material,
            Mesh = Mesh,
            ReceiveShadows = false,
            ShadowCastingMode = ShadowCastingMode.Off,
        });
    }
}