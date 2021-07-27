using System;
using Unity.Entities;

namespace DOTSRATS
{
    public struct CellStruct : IBufferElementData
    {
        public Direction wallLayout;
        public bool hole;
        public Entity goal;
        public Direction arrow;
    }
}
