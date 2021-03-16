using Unity.Entities;

namespace src.DOTS.Components
{
    public struct SwitchingPlatformTag : IComponentData
    {
        public int platformFrom;
        public int platformTo;
    }
}