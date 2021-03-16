using Unity.Entities;

namespace src.DOTS.Components
{
    [GenerateAuthoringComponent]
    public class GameObjectRefs : IComponentData
    {
        public Metro metro;
    }
}