using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace src
{
    public class LineFollowingSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float deltaTime = Time.DeltaTime;
            
            var bufferFromEntity = GetBufferFromEntity<LinePositionBufferElement>(true);
            
            return Entities
                .WithReadOnly(bufferFromEntity)
                .ForEach((ref LinePosition position) =>
                {
                    var buffer = bufferFromEntity[position.Line];
                    position.Progression += (deltaTime * LineAuthoring.k_StepSize * 500);
                    while (position.Progression >= 1.0f)
                    {
                        position.Progression -= 1.0f;
                        if (position.CurrentIndex + 1 == buffer.Length)
                        {
                            position.CurrentIndex = 0;
                        }
                        else
                        {
                            position.CurrentIndex++;
                        }
                    }
                })
                .Schedule(inputDeps);
        }
    }
}