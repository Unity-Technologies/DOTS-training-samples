using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class TornadoCubesSystem : SystemBase
{
    public float spinRate = 37;
    public float upwardSpeed = 6;

	private static float internalTime = 0.0f;
	public static float TornadoSway(float y, float t)
    {
        return math.sin(y / 5.0f + t / 4.0f) * 3.0f;
    }

    protected override void OnUpdate()
    {
        var tornadoComponent = GetSingleton<Tornado>();
		float internalDeltaTime = Time.DeltaTime;
		float internalSpinRate = spinRate;
		float internalUpwardSpeed = upwardSpeed;
		internalTime += internalDeltaTime;
		float t = internalTime;
        tornadoComponent.tornadoX = math.cos(internalTime / 6.0f) * 30.0f;
        tornadoComponent.tornadoZ = math.sin(internalTime / 6.0f * 1.618f) * 30.0f;

        Entities
            .ForEach((ref Cube tag, ref Translation translation) => 
			{
				float3 tornadoPos = new float3(tornadoComponent.tornadoX + TornadoCubesSystem.TornadoSway(translation.Value.y, t), translation.Value.y, tornadoComponent.tornadoZ);
            
				float3 delta = (tornadoPos - translation.Value);
				float dist = math.length( delta );
				delta /= dist;
				float inForce = dist - math.clamp(tornadoPos.y / 50f, 0.0f, 1.0f) * 30.0f * tag.spinningRadius + 2.0f;

				translation.Value +=  new float3(-delta.z * internalSpinRate + delta.x * inForce, internalUpwardSpeed, delta.x * internalSpinRate + delta.z * inForce) * internalDeltaTime;

				if (translation.Value.y > 50.0f) {
					translation.Value.y = 0.0f;
				}
			}).ScheduleParallel();
    }
}
