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
        dstManager.AddComponentData(entity, new Ballistic());
        dstManager.AddComponentData(entity, new TargetedBy {Value = Entity.Null});
        dstManager.AddComponentData(entity, new Velocity { Value = new float3(0,0,0) });
        dstManager.AddComponentData(entity, new AABB
            {
                center = new float3(0, 0, 0),
                halfSize = new float3(transform.localScale.x*.5f, transform.localScale.y, transform.localScale.z*0.5f)
            }
        );
    }
}