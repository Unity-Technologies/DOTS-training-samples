using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<BoardSize>();
        RequireSingletonForUpdate<WallData>();
    }
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        var boardSize = GetSingleton<BoardSize>();
        Entity wallEntity = GetSingletonEntity<WallData>();
        WallData wallData = EntityManager.GetComponentObject<WallData>(wallEntity);
        NativeArray<byte> wallArray = wallData.walls;

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
                byte curWalls = wallArray[arrayPos];
                if (!(curWalls == 0xf))
                {
                    byte newDir = (byte)direction.Value;
                    bool inFront = (curWalls & (1 << newDir)) != 0;

                    if (inFront)
                    {
                        byte rightTurn = (byte)((newDir + 1) % 4);
                        inFront = (curWalls & (1 << rightTurn)) != 0;
                        if (inFront)
                        {
                            byte leftTurn = (byte)((newDir == 0) ? 3 : newDir - 1);
                            inFront = (curWalls & (1 << leftTurn)) != 0;
                            if (inFront)
                            {
                                newDir = (byte)((newDir + 2) % 4);
                            }
                            else
                            {
                                newDir = leftTurn;
                            }
                        }
                        else
                        {
                            newDir = rightTurn;
                        }
                    }
                    direction.Value = (DirectionEnum)newDir;
                }
            }

        }).ScheduleParallel();
    }
}
