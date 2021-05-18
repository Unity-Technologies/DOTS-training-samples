using Unity.Entities;
using Unity.Rendering;

public class AntAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Ant>(entity);
        dstManager.AddComponent<Direction>(entity);
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(entity);
    }
}
