using Unity.Entities;

namespace AutoFarmers
{
    enum CellType : byte
    {
        Raw,
        Tilled,
        Rock,
        Plant,
        HarvestablePlant,
        Shop
    }
    
    struct CellTypeElement : IBufferElementData
    {
        public CellType Value;

        public CellTypeElement(CellType value)
        {
            Value = value;
        }
    }
}