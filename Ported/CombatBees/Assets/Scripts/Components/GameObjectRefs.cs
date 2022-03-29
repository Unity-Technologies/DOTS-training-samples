using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    public class GameObjectRefs : IComponentData
    {
        public UnityEngine.Camera Camera;
        public Entity ResourcePrefab;
    }
}