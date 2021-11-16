using Unity.Entities;
using UnityEngine;

namespace Dots
{
    [GenerateAuthoringComponent]
    public class GameObjectRefs : IComponentData
    {
        public Camera Camera;
        public GameObject Ground;
    }
}

