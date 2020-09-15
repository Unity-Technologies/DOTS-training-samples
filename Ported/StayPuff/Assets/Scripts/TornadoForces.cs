using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEditor.UIElements;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TornadoForces : SystemBase
{
    protected override void OnUpdate()
    {
        NativeList<TornadoForceData> TornadoForces = new NativeList<TornadoForceData>(Allocator.TempJob);
        NativeList<float3> TornadoPosition = new NativeList<float3>(Allocator.TempJob);
        float deltatime = UnityEngine.Time.deltaTime;

        Entities.ForEach((ref TornadoForceData forcedata, in Translation tornadopos ) => {
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
                float3 separation = TornadoPosition[i] - translation.Value;
                float3 inward_dir = math.normalize(separation);
                float distance = math.length(separation);
                float forcestrength = math.saturate((TornadoForces[i].tornadoMaxForceDist - distance) /TornadoForces[i].tornadoMaxForceDist);
                velocity.Linear += inward_dir * TornadoForces[i].tornadoInwardForce * mass.InverseMass * deltatime * forcestrength;

            }


        }).Schedule();

    }
}
