using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRATS
{
    public static class Utils
    {
        public static Direction GetOppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Direction.South;
                case Direction.South:
                    return Direction.North;
                case Direction.West:
                    return Direction.East;
                case Direction.East:
                    return Direction.West;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
            }

            return direction;
        }
    
        public static Direction GetNextCardinalCW(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Direction.East;
                case Direction.East:
                    return Direction.South;
                case Direction.South:
                    return Direction.West;
                case Direction.West:
                    return Direction.North;
            }

            return direction;
        }

        public static Direction GetRandomCardinalDirection(ref Random random)
        {
            return (Direction)(1 << random.NextInt(0, 4));
        }
    }
}
