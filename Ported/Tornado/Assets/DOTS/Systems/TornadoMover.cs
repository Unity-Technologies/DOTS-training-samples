using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Dots
{
    public partial class TornadoMover : SystemBase
    {
        protected override void OnUpdate()
        {
            var elapsedTime = Time.ElapsedTime;
            Entities
                .ForEach((ref Translation translation, in TornadoConfig config) =>
                {
                    var tmod = (float)elapsedTime / 6f;
                    translation.Value.xz = new float2(
                        config.initialPosition.x + math.cos(tmod) * config.rotationModulation, 
                        config.initialPosition.z + math.sin(tmod * 1.618f) * config.rotationModulation);
                }).Schedule();
        }
    }
}
