using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRATS
{
    public class TimerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .ForEach((ref GameState state) =>
                {
                    state.timer = math.max(0, state.timer - Time.DeltaTime);
                    var timerText = this.GetSingleton<GameObjectRefs>().timerText;
                    timerText.text = state.timer.ToString("00:00");
                }).Run();
        }
    }
}
