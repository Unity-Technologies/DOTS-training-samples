using System.Collections.Generic;
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

            // Hack to detect first ever time game is starting.
            bool firstTime = this.GetSingleton<GameObjectRefs>().gameOverText.text.Length == 0;
            if (firstTime)
            {
                Entities
                    .WithoutBurst()
                    .ForEach((ref GameState state) =>
                    {
                        state.timer = 0;
                    }).Run();
            }

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
                var gameOverText = this.GetSingleton<GameObjectRefs>().gameOverText;
                if (firstTime)
                {
                    gameOverText.text = "Ready...";
                }
                else
                {
                    List<int> winningPlayers = new List<int>();
                    int maxScore = -1;

                    Entities
                        .WithoutBurst()
                        .ForEach((in Player player) =>
                        {
                            if(player.score > maxScore)
                            {
                                winningPlayers.Clear();
                                winningPlayers.Add(player.playerNumber);
                                maxScore = player.score;
                            }
                            else if(player.score == maxScore)
                            {
                                winningPlayers.Add(player.playerNumber);
                            }
                        }).Run();

                    gameOverText.text = GetGameOverText(winningPlayers);
                }
                gameOverText.gameObject.SetActive(true);

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
                var gameOverText = this.GetSingleton<GameObjectRefs>().gameOverText;
                gameOverText.gameObject.SetActive(false);

                Entities
                    .WithStructuralChanges()
                    .WithAll<BoardSpawner>()
                    .ForEach((Entity entity) =>
                    {
                        EntityManager.RemoveComponent<Initialized>(entity);
                    }).Run();
            }
            else if (gameState.timer < 0 && (int)prevTime != (int)gameState.timer)
            {
                var gameOverText = this.GetSingleton<GameObjectRefs>().gameOverText;
                if (gameOverText.text.StartsWith("Ready"))
                    gameOverText.text = "Set...";
                else if (gameOverText.text.StartsWith("Set"))
                    gameOverText.text = "Go!";
            }
        }

        string GetGameOverText(List<int> winningPlayers)
        {
            if(winningPlayers.Count < 1)
            {
                return "Nobody won...";
            }

            string gameOverText;
            if(winningPlayers.Count > 1)
            {
                gameOverText = "Players ";
                for(int i = 0; i < winningPlayers.Count; i++)
                {
                    if (i == winningPlayers.Count - 1)
                        gameOverText += " and ";
                    else if (i > 0)
                        gameOverText += ", ";
                    gameOverText += winningPlayers[i];
                }
                gameOverText += " are tied!";
            }
            else
            {
                gameOverText = "Player " + winningPlayers[0] + " wins!";
            }

            return gameOverText;
        }
    }
}
