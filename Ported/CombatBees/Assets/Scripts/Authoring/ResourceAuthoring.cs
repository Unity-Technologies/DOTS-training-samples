using Components;
using Unity.Entities;

namespace Authoring
{
    public class ResourceAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager
            , GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<Components.Resource>(entity);
            dstManager.AddComponent<KinematicBody>(entity);
        }
    }
}