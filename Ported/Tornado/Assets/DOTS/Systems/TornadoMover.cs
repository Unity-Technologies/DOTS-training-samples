using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Dots
{
    [UpdateAfter(typeof(TornadoSpawner))]
    public partial class TornadoMover : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var elapsedTime = Time.ElapsedTime;
            
            Entities
                .WithName("TornadoMover")
                .ForEach((ref Translation translation, ref TornadoFader fader, in Entity entity, in TornadoConfig config) =>
                {
                    if (fader.value < 1.0f)
                        fader.value = math.saturate(fader.value + deltaTime / 10f);
                    
                    var tmod = (float)elapsedTime / 6f;
                    translation.Value.xz = new float2(
                        config.initialPosition.x + math.sin(tmod * 1.618f) * config.rotationModulation, 
                        config.initialPosition.z + math.cos(tmod) * config.rotationModulation);
                }).Schedule();
        }
    }
}
