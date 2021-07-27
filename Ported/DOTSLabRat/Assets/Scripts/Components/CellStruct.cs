using System;
using Unity.Entities;

namespace DOTSRATS
{
    [Flags]
    public enum Direction
    {
        None = 0,
        North = 1,
        South = North << 1,
        East = South << 1,
        West = East << 1,
        Up = West << 1,
        Down = Up << 1
    }

    public struct CellStruct : IBufferElementData
    {
        public Direction wallLayout;
        public bool hole;
        public Entity goal;
        public Direction arrow;
    }
}
