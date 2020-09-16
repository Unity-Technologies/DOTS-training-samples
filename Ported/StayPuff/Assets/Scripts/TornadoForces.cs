using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TornadoForces : SystemBase
{
    Random random;
    protected override void OnCreate()
    {
        random = new Random(999);
    }
    protected override void OnUpdate()
    {
        NativeList<TornadoForceData> TornadoForces = new NativeList<TornadoForceData>(Allocator.TempJob);
        NativeList<float3> TornadoPosition = new NativeList<float3>(Allocator.TempJob);
        float deltatime = UnityEngine.Time.fixedDeltaTime;
        float time = UnityEngine.Time.time;
        Random old_random = random;

        Entities.ForEach((ref TornadoForceData forcedata, in Translation tornadopos) => {
            forcedata.tornadoFader = math.saturate(forcedata.tornadoFader + deltatime / 10f);
            TornadoForces.Add(forcedata);
            TornadoPosition.Add(tornadopos.Value);

        }).Schedule();



        Entities
            .WithReadOnly(TornadoForces)
            .WithReadOnly(TornadoPosition)
            .WithNone<TornadoForceData>()
            .WithDisposeOnCompletion(TornadoForces)
            .WithDisposeOnCompletion(TornadoPosition)
            .ForEach((ref PhysicsVelocity velocity, in PhysicsMass mass, in Translation translation) => {
                for (int i = 0; i < TornadoForces.Length; i++)
                {
                    float3 finalForce = new float3();
                    float inwardForce = TornadoForces[i].tornadoInwardForce;
                    float upForce = TornadoForces[i].tornadoUpForce;
                    float tornadoHeight = TornadoForces[i].tornadoHeight;
                    float height = translation.Value.y;

                    float sway = math.sin(height / 5f + time / 4f) * 3f;

                    float tdx = TornadoPosition[i].x + sway - translation.Value.x;
                    float tdz = TornadoPosition[i].z - translation.Value.z;

                    float tornadoDist = math.sqrt(tdx * tdx + tdz * tdz);
                    tdx /= tornadoDist;
                    tdz /= tornadoDist;

                    float force = math.saturate(1f - tornadoDist / TornadoForces[i].tornadoMaxForceDist);
                    force *= TornadoForces[i].tornadoFader * TornadoForces[i].tornadoForce * old_random.NextFloat(TornadoForces[i].tornadoForceRand.x, TornadoForces[i].tornadoForceRand.y);//-.3f, 1.3f);
                    float yFader = math.saturate(1.0f - height / tornadoHeight);

                    
                    finalForce.y = upForce;

                    finalForce.x = -tdz + tdx * inwardForce * yFader;
                    finalForce.z = tdx + tdz * inwardForce * yFader;

                    velocity.Linear += force * finalForce * mass.InverseMass * deltatime;

                }

            }).Schedule();
    }
}