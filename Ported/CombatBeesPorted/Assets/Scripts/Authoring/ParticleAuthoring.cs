using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Components
{
    public class ParticleAuthoring: MonoBehaviour, IConvertGameObjectToEntity
    {
        public Vector3 initVelocity;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<Force>(entity);
            dstManager.AddComponent<Velocity>(entity);
            dstManager.AddComponent<VelocityScale>(entity);
            dstManager.AddComponentData(entity, new ShrinkAndDestroy() {lifetime = 0.5f});
            dstManager.AddComponentData(entity, new NonUniformScale() {Value = gameObject.transform.localScale});
            dstManager.AddComponentData(entity, new InitialScale() {Value = gameObject.transform.localScale});
            
        }
    }
}