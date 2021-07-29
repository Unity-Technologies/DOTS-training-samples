using UnityEngine;
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

        public static Direction GetRandomCardinalDirection(ref Unity.Mathematics.Random random)
        {
            return (Direction)(1 << random.NextInt(0, 4));
        }
        
        public static Vector2 WorldToCanvas(RectTransform canvasRect, Vector3 worldPosition, Camera camera)
        {
            var viewportPosition = camera.WorldToViewportPoint(worldPosition);

            return new Vector2((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
                (viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));
        }
        
        public static int2 PlayerNumberToGoalCoord(int playerNumber, int boardSize)
        {
            var halfSize = boardSize / 2;
            var midCoord = halfSize + (boardSize % 2 == 0 ? 1 : 2);
            var offsetFromCenter = halfSize / 6;

            var goalCoords = new int4(halfSize - offsetFromCenter - 1, midCoord + offsetFromCenter - 1,
                                       halfSize - offsetFromCenter - 1, midCoord + offsetFromCenter - 1);

            var playerGoal = new int2(playerNumber % 2 == 0 ? goalCoords[0] : goalCoords[1],
                                      playerNumber < 2 ? goalCoords[2] : goalCoords[3]);
            return playerGoal;
        }

        public static Direction GetCardinalDirectionFromIntDirection(int2 direction)
        {
            if (math.abs(direction.x) > math.abs(direction.y))
            {
                if (direction.x < 0)
                    return Direction.West;
                else
                    return Direction.East;
            }
            else
            {
                if (direction.y < 0)
                    return Direction.South;
                else
                    return Direction.North;
            }
        }
    }
}
