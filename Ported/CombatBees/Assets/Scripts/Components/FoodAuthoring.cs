using Unity.Entities;
using Unity.Mathematics;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityMeshRenderer = UnityEngine.MeshRenderer;

public class FoodAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, 
        GameObjectConversionSystem conversionSystem)
    {
        
        dstManager.AddComponent<Food>(entity);
        dstManager.AddComponentData(entity, new AABB
            {
                center = new float3(0, 0, 0),
                size = new float3(transform.localScale.x, transform.localScale.y*2.0f, transform.localScale.z)
            }
        );
    }
}