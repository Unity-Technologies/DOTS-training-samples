using System;

using Unity.Entities;

public class AIMakeDecisionSystem : SystemBase
{
    private Unity.Mathematics.Random random;

    protected override void OnCreate()
    {
        var sysRandom = new Random();
        this.random = new Unity.Mathematics.Random((uint)sysRandom.Next(0, int.MaxValue));
    }

    protected override void OnUpdate()
    {
        if (!HasSingleton<AIDecisionTimeInterval>())
            return;

        var interval = GetSingleton<AIDecisionTimeInterval>();
        var boardSpawner = GetSingleton<BoardCreationAuthor>();

        Entities
            .WithoutBurst()
            .ForEach((ref AIPlayerLastDecision player) =>
        {
            var timeElapsedSinceLast = new TimeSpan(DateTime.Now.Ticks - player.Value);

            var diff = interval.MaxMiliseconds - timeElapsedSinceLast.TotalMilliseconds;

            var r = this.random.NextInt((int)interval.MinMiliseconds, (int)interval.MaxMiliseconds);

            var elapsedMinimumTime = timeElapsedSinceLast.TotalMilliseconds > interval.MinMiliseconds;
            var elapsedMaximumTime = diff <= 0;
            var randomGTCurrent = r < timeElapsedSinceLast.TotalMilliseconds;

            //
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

                UnityEngine.Debug.Log($"Think {timeElapsedSinceLast.TotalMilliseconds} {r} ({x}, {y}) {dir}");
                player.Value = DateTime.Now.Ticks;
            }
        }).Run();
    }
}