using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEditor.UIElements;

public class TornadoForces : SystemBase
{
    protected override void OnUpdate()
    {
        NativeList<TornadoForceData> TornadoForces = new NativeList<TornadoForceData>(Allocator.TempJob);
        NativeList<float3> TornadoPosition = new NativeList<float3>(Allocator.TempJob);
        float deltatime = UnityEngine.Time.deltaTime;

        Dependency = Entities.ForEach((ref TornadoForceData forcedata, in Translation tornadopos ) => {
            TornadoForces.Add(forcedata);
            TornadoPosition.Add(tornadopos.Value);

        }).Schedule(Dependency);

        Dependency = Entities.WithReadOnly(TornadoForces).WithReadOnly(TornadoPosition).WithNone<TornadoForceData>().WithDisposeOnCompletion(TornadoForces).WithDisposeOnCompletion(TornadoPosition).ForEach((ref PhysicsVelocity velocity, in PhysicsMass mass, in Translation translation) => {
            for (int i = 0; i > TornadoForces.Length; i++)
            {
                float3 inward_dir = math.normalize(TornadoPosition[i] - translation.Value);
                velocity.Linear += inward_dir * TornadoForces[i].tornadoInwardForce * mass.InverseMass * deltatime;
            }


        }).Schedule(Dependency);

    }
}
