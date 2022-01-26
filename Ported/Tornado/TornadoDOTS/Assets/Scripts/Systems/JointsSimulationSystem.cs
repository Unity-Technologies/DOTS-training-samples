using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class JointsSimulationSystem : SystemBase
{
	//TODO: (from original) This fades de tornado force and it reaches maximum value after 10 seconds. Do we even need that?
	private float tornadoForceFader;
    protected override void OnUpdate()
    {
        var tornado = GetSingletonEntity<Tornado>();
        var parameters = GetComponent<TornadoSimulationParameters>(tornado);
        var tornadoDimensions = GetComponent<TornadoDimensions>(tornado);
        var tornadoPos = GetComponent<Translation>(tornado).Value;

        var dt = Time.fixedDeltaTime;
        var time = (float) Time.ElapsedTime;

        tornadoForceFader = math.clamp(tornadoForceFader + dt / 10.0f, 0, 1);

        var tornadoFader = tornadoForceFader;

        var rnd = new Random(1234);
        
        Entities
            .ForEach((ref DynamicBuffer<Joint> joints) =>
	        {
		        for (var i = 0; i < joints.Length; i++)
		        {
			        var joint = joints[i];

			        //TODO (perf): Maybe separate anchored joints in another buffer? (probably doesn't matter a whole lot)
			        if (joint.IsAnchored)
			        {
				        continue;
			        }
			        
					var start = joint.Value;

					var jointPos = joint.Value;
					var jointOldPos = joint.OldPos;

					jointOldPos.y += parameters.Gravity * dt;

					//TODO: We could use float2, or two floats, but whatever
					var td = new float3(
						tornadoPos.x + TornadoSway(jointPos.y, time) - jointPos.x,
						0,
						tornadoPos.z - jointPos.z
					);

					var tornadoXZDist = math.sqrt(td.x * td.x + td.z * td.z);
					td /= tornadoXZDist;

					if (tornadoXZDist < tornadoDimensions.TornadoRadius)
					{
						var forceScalar = (1.0f - tornadoXZDist / tornadoDimensions.TornadoRadius);
						var yFader = math.clamp(1f - jointPos.y / tornadoDimensions.TornadoHeight, 0, 1);
						forceScalar *= tornadoFader * parameters.TornadoForce * parameters.ForceMultiplyRange.RandomInRange(rnd);
						var force = new float3(
							-td.z - td.x * parameters.TornadoInwardForce*yFader,
							parameters.TornadoUpForce,
							td.x - td.z * parameters.TornadoInwardForce*yFader
						);

						jointOldPos -= (force * forceScalar);
					}

					jointPos += (jointPos - jointOldPos) * (1f - parameters.Damping);
					jointOldPos = start;

					if (jointPos.y < 0f)
					{
						jointPos.y = 0;
						jointOldPos.y = -jointPos.y;
						jointOldPos.x += (jointPos.x - jointOldPos.x) * parameters.Friction;
						jointOldPos.z += (jointPos.z - jointOldPos.z) * parameters.Friction;
					}

					joint.Value = jointPos;
					joint.OldPos = jointOldPos;
					joints[i] = joint;
		        }
	        }).ScheduleParallel();
    }
    
	private static float TornadoSway(float y, float time) {
		return math.sin(y / 5f + time/4f) * 3f;
	}
}