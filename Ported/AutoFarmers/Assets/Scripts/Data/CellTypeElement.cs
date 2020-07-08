using Unity.Entities;

namespace AutoFarmers
{
    enum CellType : byte
    {
        Raw,
        Tilled,
        Rock,
        Plant,
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