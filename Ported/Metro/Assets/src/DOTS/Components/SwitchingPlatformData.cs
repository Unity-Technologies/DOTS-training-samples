using Unity.Entities;

namespace src.DOTS.Components
{
    public struct SwitchingPlatformData : IComponentData
    {
        public int platformFrom;
        public int platformTo;
    }
}