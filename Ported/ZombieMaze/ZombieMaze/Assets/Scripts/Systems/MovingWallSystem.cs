using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MovingWallSystem : SystemBase
{
    protected override void OnCreate()
    {
    }

    protected override void OnDestroy()
    {
    }

    protected override void OnUpdate()
    {
        var mazeSize = GetSingleton<MazeSize>().Value;

        Entities.ForEach((ref MovingWall wall, ref Position position, ref Speed speed) =>
        {
            if (--wall.Tick <= 0)
            {
                wall.Tick = wall.Speed;

                var newPos = position.Value + wall.Direction;
                var xMin = math.max(-mazeSize.x / 2, wall.Index.x - wall.Range / 2);
                var xMax = math.min(mazeSize.x / 2, wall.Index.x + wall.Range / 2);

                if (newPos.x - 0.5f > xMax || newPos.x + 0.5f < xMin)
                {
                    wall.Direction *= -1;
                    newPos.x += wall.Direction.x * 2;
                }

                position.Value = newPos;

            }
            //position.Value += wall.Direction * 100;
        })
        .Schedule();
    }
}
