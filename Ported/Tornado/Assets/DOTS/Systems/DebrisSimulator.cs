using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Dots
{
    [UpdateAfter(typeof(TornadoMover))]
    public partial class DebrisSimulator : SystemBase
    {
        protected override void OnUpdate()
        {
            var elapsedTime = Time.ElapsedTime;
            var deltaTime = Time.DeltaTime;
            
            var tornadoEntity = GetSingletonEntity<TornadoConfig>();
            var tornadoConfig = GetComponent<TornadoConfig>(tornadoEntity);
            var tornadoEntityPosition = GetComponent<Translation>(tornadoEntity).Value;
            if (!tornadoConfig.simulate)
                return;

            Entities
                .WithName("DebrisSimulator")
                .ForEach((ref Translation translation, in Debris debris) =>
                {
                    var tornadoPos = new float3(
                        tornadoEntityPosition.x + TornadoUtils.TornadoSway(translation.Value.y, (float)elapsedTime), 
                        translation.Value.y, 
                        tornadoEntityPosition.z);
                
                    var delta = tornadoPos - translation.Value;
                    var dist = math.length(delta);
                    float inForce = dist - math.saturate(tornadoPos.y / tornadoConfig.height) * tornadoConfig.maxForceDist * debris.radiusMult + 2f;

                    delta /= dist;
                    translation.Value += new float3(-delta.z * tornadoConfig.spinRate + delta.x * inForce, 
                        tornadoConfig.upwardSpeed, 
                        delta.x * tornadoConfig.spinRate + delta.z * inForce) * deltaTime;
                
                    if (translation.Value.y>tornadoConfig.height) 
                        translation.Value.y = 0f;
                }).ScheduleParallel();
        }
    }
}