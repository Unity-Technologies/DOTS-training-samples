using Unity.Entities;

namespace src.DOTS.Components
{
    public struct CurrentPathTarget : IComponentData
    {
        public int currentIndex;
    }
}