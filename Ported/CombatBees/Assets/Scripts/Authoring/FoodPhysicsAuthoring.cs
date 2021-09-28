using Unity.Entities;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityMeshRenderer = UnityEngine.MeshRenderer;

public class FoodPhysicsAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Food>(entity);
    }
}