using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequiresEntityConversion]
[AddComponentMenu("ECS Thrower/Rock")]
public class RockAuthoringComponent: MonoBehaviour,IConvertGameObjectToEntity
{
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        Vector3 scale = transform.localScale;
        float maxScale = math.max(math.max(scale.x, scale.y), scale.z);
        
        dstManager.AddComponentData(entity, new RockRadiusComponentData
        {
            value = maxScale
        });
        
        dstManager.AddComponent<RockVelocityComponentData>(entity);
    }
}
