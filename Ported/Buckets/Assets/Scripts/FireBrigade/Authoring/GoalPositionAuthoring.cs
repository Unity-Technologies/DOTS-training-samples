using FireBrigade.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FireBrigade.Authoring
{
    public class GoalPositionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new GoalPosition{Value = new float3(Random.Range(-100f,100f), 0, Random.Range(100f, -100f))});
        }
        
    }
}