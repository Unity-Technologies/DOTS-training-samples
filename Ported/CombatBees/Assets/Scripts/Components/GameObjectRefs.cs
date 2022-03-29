using Unity.Entities;
using UnityEngine;

namespace Components
{
    [GenerateAuthoringComponent]
    public class GameObjectRefs : IComponentData
    {
        public Camera Camera;
        public Entity ResourcePrefab;
    }
}