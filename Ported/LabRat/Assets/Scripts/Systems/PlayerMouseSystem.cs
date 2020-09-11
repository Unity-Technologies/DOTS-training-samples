using Unity.Entities;

namespace Assets.Scripts.Systems
{
    public class PlayerMouseSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<HumanPlayerTag>()
                .ForEach((ref MousePosition pos) =>
            {
                var mousePos = UnityEngine.Input.mousePosition;

                pos.Value.x = mousePos.x;
                pos.Value.y = mousePos.y;
            }).Run();
        }
    }
}