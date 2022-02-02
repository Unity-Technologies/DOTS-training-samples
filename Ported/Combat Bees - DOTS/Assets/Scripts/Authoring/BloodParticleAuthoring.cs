using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
public class BloodParticleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float timeToLive;
    public float3 InitialScale = new float3(0.4f, 0.4f, 0.4f); 

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Falling
        {
            timeToLive = timeToLive,
            shouldFall = true
        });
        
        dstManager.AddComponentData(entity, new Velocity
        {
            Value = float3.zero
        });
        dstManager.AddComponentData(entity, new BloodTag()
        {
            
        });

        dstManager.AddComponentData(entity, new RandomState
        {
            Value = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000))
        });
        
        dstManager.AddComponentData(entity, new NonUniformScale
        {
            Value = InitialScale
        });
    }
}
