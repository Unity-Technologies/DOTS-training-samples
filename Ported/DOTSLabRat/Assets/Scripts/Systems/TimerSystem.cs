using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRATS
{
    public class TimerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var prevTime = GetSingleton<GameState>().timer;

            Entities
                .WithoutBurst()
                .ForEach((ref GameState state) =>
                {
                    state.timer -= Time.DeltaTime;
                    var timerText = this.GetSingleton<GameObjectRefs>().timerText;
                    timerText.text = math.max(0, state.timer).ToString("00:00");
                }).Run();

            float pauseDelay = GetSingleton<BoardSpawner>().pauseBetweenMatches;
            var gameState = GetSingleton<GameState>();
            if (prevTime > 0 && gameState.timer < 0)
            {
                // Optimize using chunking.
                var ecb = new EntityCommandBuffer(Allocator.Temp);
                Entities
                    .WithAll<InPlay>()
                    .ForEach((Entity entity) =>
                    {
                        ecb.RemoveComponent<InPlay>(entity);
                        ecb.AddComponent<InPause>(entity);
                    }).Run();
                ecb.Playback(EntityManager);
                ecb.Dispose();
            }
            else if (prevTime > -pauseDelay && gameState.timer < -pauseDelay)
            {
                Entities
                    .WithStructuralChanges()
                    .WithAll<BoardSpawner>()
                    .ForEach((Entity entity) => EntityManager.RemoveComponent<Initialized>(entity))
                    .Run();
            }
        }
    }
}
