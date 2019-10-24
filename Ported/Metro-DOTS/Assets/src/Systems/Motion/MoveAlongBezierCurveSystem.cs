using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

class MoveAlongBezierCurveSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new MoveJob {dt = Time.deltaTime}.Schedule(this, inputDeps);
    }

    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.High)]
    struct MoveJob : IJobForEach<BezierCurve, Speed, Rotation, Translation, BezierTOffset>
    {
        public float dt;

        public void Execute([ReadOnly] ref BezierCurve curve,  [ReadOnly] ref Speed speed, ref Rotation rot, ref Translation pos, ref BezierTOffset t)
        {
            t.offset += speed.value * dt;
            if (t.offset >= 1)
                t.offset = 0;
            pos.Value = BezierUtils.GetPositionAtT(curve.line, t.offset);
            var rotation = BezierUtils.GetNormalAtT(curve.line, t.offset);
            rot.Value = quaternion.LookRotation( rotation, new float3(0, 1, 0));
        }
    }
}
