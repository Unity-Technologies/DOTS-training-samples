using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(IKSolverGroupSystem))]
public class UpdateTargetSystem : JobComponentSystem
{
	[BurstCompile(FloatMode = FloatMode.Fast)]
	struct UpdateTargetJob : IJobForEach_BC<BoneJoint, ArmTarget>
    {
        public float Time;
        [Unity.Collections.ReadOnly] public ComponentDataFromEntity<Translation> Positions;
        
        
	    private float PingPong(float t, float l)
	    {
		    var L = 2.0f * l;
		    var T = t % L;

		    if (t <= l)
			    return T;
		    else
			    return L - T;
	    }

	    public void Execute([Unity.Collections.ReadOnly] DynamicBuffer<BoneJoint> joints, ref ArmTarget target)
	    {
		    var t = PingPong(Time, target.Duration);
		    var theta = target.Angle * t / target.Duration;
		    var s = 0.0f;
		    var c = 0.0f;
		    math.sincos(theta, out s, out c);
		    
		    var pos = joints[0].JointPos + target.Radius* new float3(0, s, c);
		    target.Value = pos;
	    }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new UpdateTargetJob
        {
            Time = Time.time,
            Positions = GetComponentDataFromEntity<Translation>(true)
        };
        return job.Schedule(this, inputDeps);
    }
}