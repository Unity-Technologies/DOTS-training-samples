using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : SystemBase
{
    NativeArray<byte> directionLookup;
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<BoardSize>();
        RequireSingletonForUpdate<WallData>();
        RequireSingletonForUpdate<CellData>();
        int arraySize = 4 * 16; // 4 directions, 16 possible wall states
        directionLookup = new NativeArray<byte>(arraySize, Allocator.Persistent);
        // north == 0                 east == 1                south == 2               west == 3
        directionLookup[0]  = 0; directionLookup[16] = 1; directionLookup[32] = 2; directionLookup[48] = 3; // No walls
        directionLookup[1]  = 1; directionLookup[17] = 1; directionLookup[33] = 2; directionLookup[49] = 3; // North wall only
        directionLookup[2]  = 0; directionLookup[18] = 2; directionLookup[34] = 2; directionLookup[50] = 3; // East wall only
        directionLookup[3]  = 3; directionLookup[19] = 2; directionLookup[35] = 2; directionLookup[51] = 3; // North and East walls
        directionLookup[4]  = 0; directionLookup[20] = 1; directionLookup[36] = 3; directionLookup[52] = 3; // South wall only
        directionLookup[5]  = 1; directionLookup[21] = 1; directionLookup[37] = 3; directionLookup[53] = 3; // North and South walls
        directionLookup[6]  = 0; directionLookup[22] = 0; directionLookup[38] = 3; directionLookup[54] = 3; // East and South walls
        directionLookup[7]  = 3; directionLookup[23] = 3; directionLookup[39] = 3; directionLookup[55] = 3; // North, East, and South walls
        directionLookup[8]  = 0; directionLookup[24] = 1; directionLookup[40] = 2; directionLookup[56] = 0; // West wall only
        directionLookup[9]  = 1; directionLookup[25] = 1; directionLookup[41] = 2; directionLookup[57] = 2; // North and West walls
        directionLookup[10] = 0; directionLookup[26] = 2; directionLookup[42] = 2; directionLookup[58] = 0; // East and West walls
        directionLookup[11] = 2; directionLookup[27] = 2; directionLookup[43] = 2; directionLookup[59] = 2; // North, East, and West walls
        directionLookup[12] = 0; directionLookup[28] = 1; directionLookup[44] = 1; directionLookup[60] = 0; // South and West walls
        directionLookup[13] = 1; directionLookup[29] = 1; directionLookup[45] = 1; directionLookup[61] = 1; // North, South, and West walls
        directionLookup[14] = 0; directionLookup[30] = 0; directionLookup[46] = 0; directionLookup[62] = 0; // East, South, and West walls
        directionLookup[15] = 0; directionLookup[31] = 1; directionLookup[47] = 2; directionLookup[63] = 3; // All walls

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        directionLookup.Dispose();
    }
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        var boardSize = GetSingleton<BoardSize>();
        Entity wallEntity = GetSingletonEntity<WallData>();
        WallData wallData = EntityManager.GetComponentObject<WallData>(wallEntity);
        NativeArray<byte> wallArray = wallData.walls;
        var cellEntity = GetSingletonEntity<CellData>();
        var cellData = EntityManager.GetComponentObject<CellData>(cellEntity);
        NativeArray<byte> cellDirections = cellData.directions;
        var directionLUT = directionLookup;

        Entities.ForEach((ref Position position, ref PositionOffset positionOffset, ref Direction direction, in Speed speed) => {
            positionOffset.Value += speed.Value * deltaTime;
            bool newCell = false;
            if (positionOffset.Value > 1f)
            {
                positionOffset.Value -= 1f;
                var pos = position.Value;
                switch (direction.Value)
                {
                    case DirectionEnum.North:
                        pos.y += 1;
                        break;
                    case DirectionEnum.South:
                        pos.y -= 1;
                        break;
                    case DirectionEnum.East:
                        pos.x += 1;
                        break;
                    case DirectionEnum.West:
                        pos.x -= 1;
                        break;
                }
                position.Value = pos;
                newCell = true;
            }

            // If we moved into a new cell, we need to check if we're running into a wall
            // or a cell with an arrow in it.
            if (newCell)
            {
                int arrayPos = boardSize.Value.x * position.Value.y + position.Value.x;
                // Did an arrow change our direction?
                var curCellDirection = cellDirections[arrayPos];
                switch (curCellDirection)
                {
                    case 0x1:
                        direction.Value = DirectionEnum.North;
                        break;
                    case 0x2:
                        direction.Value = DirectionEnum.East;
                        break;
                    case 0x4:
                        direction.Value = DirectionEnum.South;
                        break;
                    case 0x8:
                        direction.Value = DirectionEnum.West;
                        break;
                    default:
                        break;
                }
                byte curWalls = wallArray[arrayPos];
                byte curDir = (byte)direction.Value;
                byte newDir = directionLUT[curDir * 16 + curWalls];
                direction.Value = (DirectionEnum)newDir;
            }

        }).ScheduleParallel();
    }
}
