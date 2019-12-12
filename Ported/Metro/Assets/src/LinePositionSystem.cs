using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace src
{
    public class LinePositionSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var bufferFromEntity = GetBufferFromEntity<LineSegmentBufferElement>(true);
            
            return Entities
                .WithReadOnly(bufferFromEntity)
                .ForEach((ref LinePosition position, ref Translation translation, ref Rotation rotation, in Speed speed) =>
                {
                    var buffer = bufferFromEntity[position.Line];
                    var step = buffer[position.CurrentIndex];
                    var distanceBetweenSteps = math.length(step.p3 - step.p0);

                    position.Progression += speed.Value / distanceBetweenSteps;
                    if (position.Progression >= 1.0f)
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
                        step = buffer[position.CurrentIndex];
                    }
                    translation.Value = BezierMath.GetPoint(step, position.Progression);
                    var lookDirection = math.normalize(BezierMath.GetPoint(step, position.Progression + 0.001f) - translation.Value);
                    rotation.Value = quaternion.LookRotation(lookDirection, new float3(
                        lookDirection.y * lookDirection.x / (lookDirection.x + lookDirection.z),
                        math.length(new float2(lookDirection.x, lookDirection.z)),
                        lookDirection.y * lookDirection.z / (lookDirection.x + lookDirection.z)));
                })
                .Schedule(inputDeps);
        }
    }
}