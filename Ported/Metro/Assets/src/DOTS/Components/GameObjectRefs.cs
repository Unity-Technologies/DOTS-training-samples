using Unity.Entities;
using UnityEngine;

namespace src.DOTS.Components
{
    [GenerateAuthoringComponent]
    public class GameObjectRefs : IComponentData
    {
        public Metro metro;
        public Camera camera;
    }
}