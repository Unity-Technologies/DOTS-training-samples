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

    //[BurstCompile(FloatMode = FloatMode.Fast)]
    struct MoveJob : IJobForEach<BezierCurve, Speed, Rotation, Translation, BezierTOffset>
    {
        public float dt;

        public void Execute([ReadOnly] ref BezierCurve curve,  [ReadOnly] ref Speed speed, ref Rotation rot, ref Translation pos, ref BezierTOffset t)
        {
            if (speed.value < 0.00000000001f)
                return;

            t.offset += speed.value * dt;
            var renderT = math.fmod(t.offset + t.renderOffset, 1.0f);
            pos.Value = BezierUtils.GetPositionAtT(curve.line, renderT);
            var rotation = BezierUtils.GetNormalAtT(curve.line, renderT);
            rot.Value = quaternion.LookRotation( rotation, new float3(0, 1, 0));
        }
    }
}
