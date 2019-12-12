
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public struct AntMovementJob : IJobForEach<Translation, Rotation, AntComponent>
{
	public float2 ColonyPosition;
	public float2 ResourcePosition;
	public float TargetRadius;
	public float TimeDelta;

	public void Execute(ref Translation translation, ref Rotation rotation, ref AntComponent ant)
	{
		float velocityChange = ant.acceleration * TimeDelta;
		float3 position = translation.Value;

		math.sincos(ant.facingAngle, out float sin, out float cos);
		ant.speed += velocityChange;

		float speedChange = ant.speed * TimeDelta;
		float3 velocity = new float3(speedChange * cos, speedChange * sin, 0.0f);
		position += velocity;

		if((position.x < 0) || (position.x >= 1.0f))
		{
			velocity.x = -velocity.x;
			position = translation.Value;
		} 
		if((position.y < 0.0f) || (position.y >= 1.0f))
		{
			velocity.y = -velocity.y;
			position = translation.Value;
		}

		ant.facingAngle = math.atan2(velocity.y, velocity.x);
		translation.Value = position;
		rotation.Value = quaternion.Euler(0.0f, 0.0f, ant.facingAngle);

		var targetPos = ant.state == 0 ? ResourcePosition : ColonyPosition;

		if (math.lengthsq(translation.Value.xy - targetPos) < TargetRadius * TargetRadius)
		{
			ant.state = 1 - ant.state;
			ant.facingAngle = math.fmod(ant.facingAngle + math.PI, math.PI * 2);
		}
	}
}
