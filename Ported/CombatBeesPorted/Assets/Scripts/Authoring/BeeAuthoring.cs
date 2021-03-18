using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Components
{
    public class BeeAuthoring: MonoBehaviour, IConvertGameObjectToEntity
    {
        public int TeamIndex = 0;
        
        public Vector3 initVelocity;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<Bee>(entity);
            dstManager.AddComponent<Force>(entity);
            dstManager.AddComponent<VelocityScale>(entity);
            dstManager.AddComponentData(entity, new Velocity() { Value = initVelocity});
            
            dstManager.AddComponentData(entity, new Team() { index = TeamIndex == 0});

            dstManager.AddComponentData(entity, new InitialScale() {Value = gameObject.transform.localScale});

            if (TeamIndex == 0)
                dstManager.AddComponent<TeamA>(entity);
            else
                dstManager.AddComponent<TeamB>(entity);
        }
    }
}