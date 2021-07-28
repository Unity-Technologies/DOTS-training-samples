using Unity.Entities;

namespace DOTSRATS
{
    public class ScoreDisplaySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .ForEach((in Player player) =>
                {
                    var timerText = this.GetSingleton<GameObjectRefs>().playerScore[player.playerNumber];
                    timerText.text = player.score.ToString();
                }).Run();
        }
    }
}