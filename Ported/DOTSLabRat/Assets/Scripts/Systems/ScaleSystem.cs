using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

using Mathf = UnityEngine.Mathf;

namespace DOTSRATS
{
    public class ScaleSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities
                .ForEach((Entity entity, ref Scale currentScale, ref Scaling scaling) =>
                {
                    if(currentScale.Value != scaling.targetScale)
                    {
                        currentScale.Value = Mathf.Lerp(currentScale.Value, scaling.targetScale, deltaTime*5);
                    }
                }).ScheduleParallel();
        }
    }
}
