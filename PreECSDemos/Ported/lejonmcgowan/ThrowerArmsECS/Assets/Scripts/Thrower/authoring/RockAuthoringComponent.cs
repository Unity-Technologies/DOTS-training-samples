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
        uint seed = 0x2048;
        seed <<= entity.Index % 19;
        dstManager.AddComponentData(entity, new RockRadiusComponentData
        {
            Value = maxScale
        });
        
        dstManager.AddComponentData(entity, new RockCollisionRNG()
        {
            Value = new Unity.Mathematics.Random(seed)
        });
        
        dstManager.AddComponent<Velocity>(entity);
    }
}
