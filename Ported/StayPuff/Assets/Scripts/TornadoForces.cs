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
    EntityQuery TornadoListQuery;
    Random SystemRandom;

    protected override void OnCreate()
    {
        TornadoListQuery = GetEntityQuery(typeof(TornadoForceData), ComponentType.ReadOnly<Translation>());
        SystemRandom = new Random(999);
    }

    protected override void OnUpdate()
    {
        int numTornados = TornadoListQuery.CalculateEntityCount();
        NativeList<TornadoForceData> tornadoForces = new NativeList<TornadoForceData>(numTornados, Allocator.TempJob);
        NativeList<float3> tornadoPosition = new NativeList<float3>(numTornados, Allocator.TempJob);
        NativeList<TornadoForceData>.ParallelWriter tornadoForcesWriter = tornadoForces.AsParallelWriter();
        NativeList<float3>.ParallelWriter tornadoPositionWriter = tornadoPosition.AsParallelWriter();

        float deltatime = UnityEngine.Time.fixedDeltaTime;
        float time = UnityEngine.Time.time;
        Random jobRandom = SystemRandom;

        Entities
            .ForEach((ref TornadoForceData forcedata, in Translation tornadopos) => {
                forcedata.tornadoFader = math.saturate(forcedata.tornadoFader + deltatime / 10f);
                tornadoForcesWriter.AddNoResize(forcedata);
                tornadoPositionWriter.AddNoResize(tornadopos.Value);
            }).ScheduleParallel();

        Entities
            .WithReadOnly(tornadoForces)
            .WithReadOnly(tornadoPosition)
            .WithNone<TornadoForceData>()
            .WithDisposeOnCompletion(tornadoForces)
            .WithDisposeOnCompletion(tornadoPosition)
            .ForEach((ref PhysicsVelocity velocity, in PhysicsMass mass, in Translation translation) => {
                for (int i = 0; i < tornadoForces.Length; i++)
                {
                    float3 finalForce = new float3();
                    float inwardForce = tornadoForces[i].tornadoInwardForce;
                    float upForce = tornadoForces[i].tornadoUpForce;
                    float tornadoHeight = tornadoForces[i].tornadoHeight;
                    float height = translation.Value.y;

                    float sway = math.sin(height / 5f + time / 4f) * 3f;

                    float tdx = tornadoPosition[i].x + sway - translation.Value.x;
                    float tdz = tornadoPosition[i].z - translation.Value.z;

                    float tornadoDist = math.sqrt(tdx * tdx + tdz * tdz);
                    tdx /= tornadoDist;
                    tdz /= tornadoDist;

                    float force = math.saturate(1f - tornadoDist / tornadoForces[i].tornadoMaxForceDist);
                    force *= tornadoForces[i].tornadoFader * tornadoForces[i].tornadoForce * jobRandom.NextFloat(tornadoForces[i].tornadoForceRand.x, tornadoForces[i].tornadoForceRand.y);//-.3f, 1.3f);
                    float yFader = math.saturate(1.0f - height / tornadoHeight);

                    finalForce.y = upForce;

                    finalForce.x = -tdz + tdx * inwardForce * yFader;
                    finalForce.z = tdx + tdz * inwardForce * yFader;

                    velocity.Linear += force * finalForce * mass.InverseMass * deltatime;
                }
            }).ScheduleParallel();
    }
}