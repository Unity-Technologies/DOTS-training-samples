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
            Entities
                .WithoutBurst()
                .ForEach((Entity entity, ref Scale currentScale, ref Scaling targetScale) =>
                {
                    if(currentScale.Value != targetScale.targetScale)
                    {
                        currentScale.Value = Mathf.Lerp(currentScale.Value, targetScale.targetScale, targetScale.currentInterpolation);
                        targetScale.currentInterpolation = math.max(targetScale.interpolationMax, targetScale.currentInterpolation + targetScale.interpolationRate);
                        if (targetScale.currentInterpolation == targetScale.interpolationMax)
                            targetScale.currentInterpolation = 0;
                    }
                    
                }).Run();
        }
    }
}
