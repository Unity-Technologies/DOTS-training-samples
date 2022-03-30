using Components;
using Unity.Entities;

namespace Authoring
{
    public class ResourceSpawnerAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.DestroyEntity(entity);
            
            var newEntity = dstManager.CreateEntity();
            dstManager.AddComponent<ResourceSpawner>(newEntity);
        }
    }
}