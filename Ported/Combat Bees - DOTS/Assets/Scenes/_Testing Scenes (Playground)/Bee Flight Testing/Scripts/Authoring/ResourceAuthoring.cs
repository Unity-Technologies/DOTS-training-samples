using Unity.Entities;
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
                Value =  Entity.Null
            });
            
            dstManager.AddComponentData(entity, new ResourceState
            {
                Free =  true
            });
        }
    }
}