using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Dots
{
    public partial class TornadoMoverSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var time = Time.ElapsedTime;
            Entities
                .WithAll<Simulated>()
                .ForEach((ref Tornado tornado) =>
            {
                float tmod = (float)time / 6f;
                tornado.position.x = tornado.initialPosition.x + Mathf.Cos(tmod) * tornado.rotationModulation;
                tornado.position.z = tornado.initialPosition.z + Mathf.Sin(tmod * 1.618f) * tornado.rotationModulation;
            }).ScheduleParallel();
        }
    }
}

