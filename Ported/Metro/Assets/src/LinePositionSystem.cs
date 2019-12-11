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
            var bufferFromEntity = GetBufferFromEntity<LinePositionBufferElement>(true);
            
            return Entities
                .WithReadOnly(bufferFromEntity)
                .ForEach((ref LinePosition position, ref Translation translation) =>
                {
                    var buffer = bufferFromEntity[position.Line];
                    var step = buffer[position.CurrentIndex];
                    var nextStep = position.CurrentIndex + 1 < buffer.Length
                        ? buffer[position.CurrentIndex + 1]
                        : buffer[0];
                    translation.Value = math.lerp(step.Value, nextStep.Value, position.Progression);
                    
                })
                .Schedule(inputDeps);
        }
    }
}