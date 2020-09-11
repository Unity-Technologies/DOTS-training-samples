using System;

using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

public class AIMakeDecisionSystem : SystemBase
{
    private Unity.Mathematics.Random random;
    private EntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        var sysRandom = new System.Random();
        this.random = new Unity.Mathematics.Random((uint)sysRandom.Next(0, int.MaxValue));
        this.ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (!HasSingleton<AIDecisionTimeInterval>())
            return;

        var ecb = this.ecbSystem.CreateCommandBuffer();
        var interval = GetSingleton<AIDecisionTimeInterval>();
        var boardSpawner = GetSingleton<BoardCreationAuthor>();

        Entities
            .WithNone<AIPlayerMovingCursorTag>()
            .WithStructuralChanges()
            .ForEach((Entity playerEntity, ref AIPlayerLastDecision player) =>
        {
            var timeElapsedSinceLast = new TimeSpan(DateTime.Now.Ticks - player.Value);

            var diff = interval.MaxMiliseconds - timeElapsedSinceLast.TotalMilliseconds;

            var r = this.random.NextInt(interval.MinMiliseconds, interval.MaxMiliseconds);

            var elapsedMinimumTime = timeElapsedSinceLast.TotalMilliseconds > interval.MinMiliseconds;
            var elapsedMaximumTime = diff <= 0;
            var randomGTCurrent = r < timeElapsedSinceLast.TotalMilliseconds;

            if (elapsedMinimumTime && (elapsedMaximumTime || randomGTCurrent))
            {
                var x = this.random.NextInt(0, boardSpawner.SizeX - 1);
                var y = this.random.NextInt(0, boardSpawner.SizeY - 1);

                var dirInt = this.random.NextInt(0, 3);

                Direction.Attributes dir;

                switch (dirInt)
                {
                    case 0:
                        dir = Direction.Attributes.Up;
                        break;

                    case 1:
                        dir = Direction.Attributes.Right;
                        break;

                    case 2:
                        dir = Direction.Attributes.Down;
                        break;

                    default:
                    case 3:
                        dir = Direction.Attributes.Left;
                        break;
                }

                ecb.AddComponent(playerEntity, new AIPlayerMovingCursorTag());

                player.Value = DateTime.Now.Ticks;

                var screenPos = Camera.main.WorldToScreenPoint(new Vector3(x, 0, y));

                //UnityEngine.Debug.Log(screenPos);

                ecb.SetComponent(playerEntity, new AIMousePositionTarget
                {
                    Value = new float2(screenPos.x, screenPos.y),
                    Tile = new int2(x, y),
                    Direction = dir
                });
            }
        }).Run();
    }
}