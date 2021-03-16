using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Components
{
    public class BeeAuthoring: MonoBehaviour, IConvertGameObjectToEntity
    {
        public int TeamIndex = 0;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<Bee>(entity);
            dstManager.AddComponent<Force>(entity);
            dstManager.AddComponentData(entity, new Velocity() { Value = new float3(2, 3, 4)});
            
            dstManager.AddComponentData(entity, new Team() { index = TeamIndex == 0});

            if (TeamIndex == 0)
                dstManager.AddComponent<TeamA>(entity);
            else
                dstManager.AddComponent<TeamB>(entity);
        }
    }
}