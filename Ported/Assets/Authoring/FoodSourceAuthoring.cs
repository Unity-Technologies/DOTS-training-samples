using Unity.Entities;
using Unity.Rendering;

public class FoodSourceAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Respawn>(entity);
        dstManager.AddComponent<FoodSource>(entity);
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(entity);
    }
}
