using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public partial class ParticleSystem : SystemBase
    {
        const float spinRate = 37.0f;
        const float upwardSpeed = 6.0f;

        private static float TornadoSway(float y, float timeElapsed)
        {
            return math.sin(y / 5f + timeElapsed/4f) * 3f;
        }
        protected override void OnUpdate()
        {
            var deltaTime = UnityEngine.Time.deltaTime;
            var timeElapsed = UnityEngine.Time.time;

            Entities
                .ForEach((ref Translation translation, in Particle particle) =>
                {
                    var initialPosition = translation.Value;
                    var tornadoPos = new float3(0.0f + TornadoSway(initialPosition.y, timeElapsed), initialPosition.y, 0.0f);
                    //Vector3 tornadoPos = new Vector3(PointManager.tornadoX + PointManager.TornadoSway(points[i].y), points[i].y, PointManager.tornadoZ);
                    var delta = (tornadoPos - initialPosition);
                    float dist = math.length(delta);
                    delta /= dist;
                    float inForce = dist - math.clamp(tornadoPos.y / 50f, 0.0f, 1.0f) * 30.0f * particle.radiusMult + 2f;
                    var velocity = new float3(-delta.z, 0.0f, delta.x) * spinRate
                                   + delta * inForce
                                   + new float3(0.0f, upwardSpeed, 0.0f);
                    var nextPosition = initialPosition + velocity * deltaTime;

                    if (nextPosition.y > 50f)
                    {
                        nextPosition.y = 0.0f;
                    }

                    translation.Value = nextPosition;
                }).ScheduleParallel();
        }
    }
}