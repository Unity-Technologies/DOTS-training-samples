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
	    //TODO: Turning it off until we have constraint resolution
	    return;
        var tornado = GetSingletonEntity<TornadoSimulationParameters>();
        var parameters = GetComponent<TornadoSimulationParameters>(tornado);
        var tornadoPos = GetComponent<Translation>(tornado).Value;

        var dt = Time.fixedDeltaTime;
        var time = (float) Time.ElapsedTime;

        tornadoForceFader = math.clamp(tornadoForceFader + dt / 10.0f, 0, 1);

        var tornadoFader = tornadoForceFader;

        var rnd = new Random(1234);
        
        Entities
	        .WithAll<Point>()
	        .ForEach((ref Translation translation, ref PointOldTranslation oldTranslation) =>
	        {
		        var start = translation.Value;

		        var jointPos = translation.Value;
		        var jointOldPos = oldTranslation.Value;

		        jointOldPos.y += parameters.Gravity * dt;

		        //TODO: We could use float2, or two floats, but whatever
		        var td = new float3(
			        tornadoPos.x + TornadoSway(jointPos.y, time) - jointPos.x,
			        0,
			        tornadoPos.z - jointPos.z
		        );

		        var tornadoXZDist = math.sqrt(td.x * td.x + td.z * td.z);
		        td /= tornadoXZDist;

		        if (tornadoXZDist < parameters.TornadoMaxForceDist)
		        {
			        var forceScalar = (1.0f - tornadoXZDist / parameters.TornadoMaxForceDist);
			        var yFader = math.clamp(1f - jointPos.y / parameters.TornadoHeight, 0, 1);
			        forceScalar *= tornadoFader * parameters.TornadoForce * parameters.ForceMultiplyRange.RandomInRange(rnd);
			        var force = new float3(
				        -td.z + td.x * parameters.TornadoInwardForce*yFader,
				        parameters.TornadoUpForce,
				        td.x + td.z * parameters.TornadoInwardForce*yFader
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

		        translation.Value = jointPos;
		        oldTranslation.Value = jointOldPos;
	        }).ScheduleParallel();
    }
    
	private static float TornadoSway(float y, float time) {
		return math.sin(y / 5f + time/4f) * 3f;
	}
}