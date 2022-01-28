using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class BloodParticleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{


    public float timeToLive;
    public float steps;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BeeBloodParticle
        {
            timeToLive = timeToLive,
        });
        
        dstManager.AddComponentData(entity, new Velocity
        {
            Value = float3.zero
        });
        
    }
  
}
