using Unity.Entities;

namespace AutoFarmers
{
    struct CellEntityElement : IBufferElementData
    {
        public Entity Value;

        public CellEntityElement(Entity value)
        {
            Value = value;
        }
    }
}