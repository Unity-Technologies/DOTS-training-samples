using Unity.Entities;
using Unity.Mathematics;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

namespace CombatBees.Testing.BeeFlight
{
    public class ResourceAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<Resource>(entity);
            dstManager.AddComponent<Holder>(entity);
            dstManager.AddComponent<ResourceState>(entity);
            
            dstManager.AddComponentData(entity, new Holder
            {
                BeeEntity =  Entity.Null,
                Offset = new float3(0f, -1f, 0f)
            });
            
            dstManager.AddComponentData(entity, new ResourceState
            {
                Free =  true
            });
        }
    }
}