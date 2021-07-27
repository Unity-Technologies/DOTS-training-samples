using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(BeeSimulationSystem))]
[UpdateAfter(typeof(ResourceSimulationSystem))]
class TranslationUpdateSystem: SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((ref Translation pos, in NewTranslation newPos) =>
            {
                pos = newPos.translation;
            }).ScheduleParallel();
    }
}
