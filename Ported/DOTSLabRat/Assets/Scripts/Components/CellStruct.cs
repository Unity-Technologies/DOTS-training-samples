using System;
using Unity.Entities;

namespace DOTSRATS
{
    [Flags]
    public enum Direction
    {
        North = 0x0001,
        South = 0x0002,
        East = 0x0004,
        West = 0x0008
    }

    public struct CellStruct : IBufferElementData
    {
        public Direction wallLayout;
        public bool hole;
        // public EntityRef goal;
        public Direction arrow;
    }
}
