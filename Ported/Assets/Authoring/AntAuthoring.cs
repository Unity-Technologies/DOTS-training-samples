using Unity.Entities;
using Unity.Rendering;

public class AntAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Position>(entity);
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(entity);
    }
}
