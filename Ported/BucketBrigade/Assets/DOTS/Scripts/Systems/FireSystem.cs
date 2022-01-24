using Unity.Entities;
using Unity.Rendering;

public partial class FireSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref FireField fireField) =>
        {
            var field = GetBuffer<FireHeat>(e);

        }).Schedule();
    }
}
