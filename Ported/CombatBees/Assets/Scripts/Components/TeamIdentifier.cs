using Unity.Entities;

namespace Components
{
    public partial struct TeamIdentifier : ISharedComponentData
    {
        public int TeamNumber;
    }
}