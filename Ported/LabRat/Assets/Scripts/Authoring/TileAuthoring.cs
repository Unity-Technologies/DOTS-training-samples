using Unity.Entities;
using Unity.Rendering;
using MonoBehaviour = UnityEngine.MonoBehaviour;

public class TileAuthoring: MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(entity);
        dstManager.AddComponent<Tile>(entity);
        //dstManager.AddComponent<Direction>(entity);
    }
}
