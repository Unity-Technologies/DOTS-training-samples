using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace src
{
    public class LinePositionSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var bufferFromEntity = GetBufferFromEntity<LineSegmentBufferElement>(true);
            
            return Entities
                .WithReadOnly(bufferFromEntity)
                .ForEach((ref LinePosition position, ref Translation translation, in Speed speed) =>
                {
                    var buffer = bufferFromEntity[position.Line];
                    var step = buffer[position.CurrentIndex];
                    var distanceBetweenSteps = math.length(step.p3 - step.p0);

                    position.Progression += speed.Value / distanceBetweenSteps;
                    while (position.Progression >= 1.0f)
                    {
                        position.Progression -= 1.0f;
                        if (position.CurrentIndex + 1 >= buffer.Length)
                        {
                            position.CurrentIndex = 0;
                        }
                        else
                        {
                            position.CurrentIndex++;
                        }
                    }
                    translation.Value = BezierMath.GetPoint(step, position.Progression);
                    
                })
                .Schedule(inputDeps);
        }
    }
}