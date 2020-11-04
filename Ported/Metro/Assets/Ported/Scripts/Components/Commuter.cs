using Unity.Entities;

namespace MetroECS.Comuting
{
    public struct Commuter : IComponentData
    {
        public Entity Seat;
    }
}