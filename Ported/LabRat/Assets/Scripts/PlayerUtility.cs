using System;
using Unity.Mathematics;
using Unity.Rendering;

static class PlayerUtility
{
    public static URPMaterialPropertyBaseColor ColorFromPlayerIndex(int index)
    {
        URPMaterialPropertyBaseColor color;
        switch (index)
        {
            case 1: // Red
                color = new URPMaterialPropertyBaseColor {Value = new float4(1.0f, 0.0f, 0.0f, 1.0f)};
                break;
            case 2: // Green
                color = new URPMaterialPropertyBaseColor {Value = new float4(0.0f, 1.0f, 0.0f, 1.0f)};
                break;
            case 3: // Blue
                color = new URPMaterialPropertyBaseColor {Value = new float4(0.0f, 0.0f, 1.0f, 1.0f)};
                break;
            default: // Black
                color = new URPMaterialPropertyBaseColor {Value = new float4(0.0f, 0.0f, 0.0f, 1.0f)};
                break;
        }

        return color;
    }

    // Never tested, but:
    // Could be used to pre-compute the wall positions and then use a job to parallelize the actual creation
    // of the board. Basically, all cell can be computed independently after that as only the walls are "shared"
    // between cell.
    public static byte[] ComputeWallPositions(int boardSize, float wallPercentage = 0.5f)
    {
        var walls = new byte[boardSize * boardSize];

        var maxNbWalls = boardSize * boardSize * 3 - boardSize * 4;
        var wallCount = maxNbWalls * wallPercentage;
        var wallProbability = wallCount / (boardSize * boardSize);

        var rand = new Unity.Mathematics.Random((uint) DateTime.Now.Ticks);

        for (var i = 0; i < boardSize; i++)
        for (var j = 0; j < boardSize; j++)
        {
            var index = j * boardSize + i;

            if (i == 0)
                walls[index] ^= DirectionDefines.West;
            else if (i == boardSize - 1)
                walls[index] ^= DirectionDefines.East;

            if (j == 0)
                walls[index] ^= DirectionDefines.South;
            else if (j == boardSize - 1)
                walls[index] ^= DirectionDefines.North;

            if (!(wallCount > 0) || walls[index] >= 15 || !(rand.NextFloat(1f) < wallProbability))
                continue;

            uint wallDirection = 0x1;
            while (wallDirection != 0)
            {
                wallDirection = 2 ^ rand.NextUInt(0, 3);
                wallDirection = walls[index] & wallDirection;
            }

            switch (wallDirection)
            {
                case DirectionDefines.North:
                    var northIndex = (j < boardSize - 1) ? (j + 1) * boardSize + i : -1;
                    if (northIndex > 0 && walls[northIndex] < 15)
                    {
                        walls[index] ^= DirectionDefines.North;
                        walls[northIndex] ^= DirectionDefines.South;
                    }
                    break;

                case DirectionDefines.South:
                    var southIndex = (j > 0) ? (j - 1) * boardSize + i : -1;
                    if (southIndex > 0 && walls[southIndex] < 15)
                    {
                        walls[index] ^= DirectionDefines.South;
                        walls[southIndex] ^= DirectionDefines.North;
                    }
                    break;

                case DirectionDefines.West:
                    var westIndex = (i > 0) ? j * boardSize + (i - 1) : -1;
                    if (westIndex > 0 && walls[westIndex] < 15)
                    {
                        walls[index] ^= DirectionDefines.West;
                        walls[westIndex] ^= DirectionDefines.East;
                    }
                    break;

                case DirectionDefines.East:
                    var eastIndex = (i < boardSize - 1) ? j * boardSize + (i + 1) : -1;
                    if (eastIndex > 0 && walls[eastIndex] < 15)
                    {
                        walls[index] ^= DirectionDefines.East;
                        walls[eastIndex] ^= DirectionDefines.West;
                    }
                    break;

                default:
                    break;
            }
        }

        return walls;
    }
}
