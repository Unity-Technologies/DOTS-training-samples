using Unity.Entities;

namespace Components
{
    public struct PassengerTravel: IComponentData
    {
        public int LineID;
        public bool OnPlatformA;
        public Entity Station;
        public Entity Queue; // switch to target location if possible
    }
}