using Unity.Mathematics;

public class Util
{
    public static void PositionToCoordinates(float3 position, BoardDataComponent board, out float2 cellCoord, out int cellIndex)
    {
        var localPt = new float2(position.x, position.z);
        localPt += board.cellSize * 0.5f; // offset by half cellsize
        cellCoord = new float2(math.floor(localPt.x / board.cellSize.x), math.floor(localPt.y / board.cellSize.y));
        cellIndex = (int)(cellCoord.y * board.size.x + cellCoord.x);
    }
}