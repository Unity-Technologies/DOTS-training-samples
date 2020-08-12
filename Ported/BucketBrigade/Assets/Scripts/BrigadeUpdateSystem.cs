using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BrigadeUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // move the bots towards their targets
        var deltaTime = Time.DeltaTime;
        Entities
            .WithAll<TargetPosition>()
            .ForEach((ref Translation translation, in TargetPosition target) =>
            {
                translation.Value =
                    translation.Value + math.normalize(target.Value - translation.Value) * 1 * deltaTime;
            }).Schedule();
    }
}